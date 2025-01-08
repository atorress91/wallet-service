using System.Reflection;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services;

public class HouseCoinPdfService : IHouseCoinPdfService
{
    public async Task<byte[]> GenerateInvoice(UserInfoResponse userInfo, DebitTransactionRequest invoice,
        InvoicesSpResponse spResponse)
    {
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        var totalTax = 0m;
        var subTotal = 0m;
        var totalDiscount = 0m;
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator = Path.DirectorySeparatorChar;
        var pathFile = $"{workingDirectory}{separator}Assets{separator}house-coin-logo.png";

        return await Task.Run(() =>
        {
            var document = Document
                .Create(doc =>
                {
                    doc.Page(page =>
                    {
                        page.Margin(15);
                        page.PageColor("#0A192F");

                        page.Header().ShowOnce().Row(row =>
                        {
                            var image = Image.FromFile(pathFile);
                            row.ConstantItem(170).Container().Image(image);

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight()
                                    .Background("#64FFDA")
                                    .Padding(10).Border(1).BorderColor("#CCD6F6")
                                    .AlignCenter()
                                    .Text($"Recibo #{spResponse.Id}")
                                    .FontColor("#0A192F");

                                col.Item().AlignRight().Text("SMART INTERNATIONAL INVESTMENTS LLC")
                                    .FontColor("#CCD6F6").FontSize(10);
                                col.Item().AlignRight().Text("L24000157429").FontColor("#8892B0").FontSize(10);
                                col.Item().AlignRight()
                                    .Text("Domicilio: España")
                                    .FontColor("#8892B0").FontSize(10);
                                col.Item().AlignRight().Text("Tel/Fax: (561) 694-8107").FontColor("#8892B0")
                                    .FontSize(10);
                                col.Item().AlignRight().Text("Correo: support@thehousecoin.net").FontColor("#8892B0")
                                    .FontSize(10);
                                col.Item().AlignRight().Text(date).FontColor("#8892B0").FontSize(10);
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col1 =>
                        {
                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignLeft().Text("Datos del cliente").Underline().Bold()
                                        .FontColor("#64FFDA");

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Nombre: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span($"{userInfo.Name} {userInfo.LastName}").FontColor("#8892B0")
                                            .FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("País: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Country!.Name).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Ciudad: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.City).FontColor("#8892B0").FontSize(10);
                                    });
                                });

                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Teléfono: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Phone).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("No de Identificación fiscal: ").SemiBold().FontSize(10)
                                            .FontColor("#CCD6F6");
                                        txt.Span(userInfo.UserName).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Correo: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Email).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Domicilio: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Address).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        if (!string.IsNullOrEmpty(spResponse.Reason))
                                        {
                                            txt.Span("Referencia: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                            txt.Span(spResponse.Reason).FontColor("#8892B0").FontSize(10);
                                        }
                                    });
                                });
                            });

                            col1.Item().LineHorizontal(0.5f).LineColor("#64FFDA");

                            col1.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Concepto")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Cantidad")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Precio")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Descuento")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Total")
                                        .FontColor("#0A192F");
                                });

                                foreach (var item in invoice.invoices)
                                {
                                    var conceptName = item.ProductName;
                                    var quantity = item.ProductQuantity;
                                    var price = item.ProductPrice;
                                    var tax = item.ProductIva;
                                    var discount = item.ProductDiscount * quantity;
                                    var total = quantity * price;

                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text(conceptName).FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text(quantity.ToString()).FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text($"$ {price:0.##}").FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text($"$ {discount:0.##}").FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text($"$ {total:0.##}").FontColor("#CCD6F6").FontSize(10);

                                    subTotal += total;
                                    totalDiscount += discount;
                                    totalTax = tax;
                                }
                            });

                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text("Monto pagado").Bold().FontSize(16)
                                        .FontColor("#64FFDA");
                                    col2.Item().AlignRight().Text($"Total: ${invoice.Debit:0.00}")
                                        .FontSize(18).FontColor("#64FFDA");
                                    col2.Item().AlignRight().Text("Fecha de pago:").FontSize(12).FontColor("#CCD6F6");
                                    col2.Item().AlignRight().Text(date).FontSize(12).FontColor("#CCD6F6");
                                });
                            });

                            col1.Spacing(10);
                        });

                        page.Footer()
                            .AlignRight()
                            .Text(txt =>
                            {
                                txt.Span("Página ").FontSize(10).FontColor("#8892B0");
                                txt.CurrentPageNumber().FontSize(10).FontColor("#CCD6F6");
                                txt.Span(" de ").FontSize(10).FontColor("#8892B0");
                                txt.TotalPages().FontSize(10).FontColor("#CCD6F6");
                            });
                    });
                });

            byte[] pdfBytes = document.GeneratePdf();
            using var memoryStream = new MemoryStream(pdfBytes);
            return memoryStream.ToArray();
        });
    }

    public async Task<byte[]> RegenerateInvoice(UserInfoResponse userInfo, Invoice invoice)
    {
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        var totalTax = 0m;
        var subTotal = 0m;
        decimal? totalDiscount = 0m;
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator = Path.DirectorySeparatorChar;
        var pathFile = $"{workingDirectory}{separator}Assets{separator}house-coin-logo.png";

        return await Task.Run(() =>
        {
            var document = Document
                .Create(doc =>
                {
                    doc.Page(page =>
                    {
                        page.Margin(15);
                        page.PageColor("#0A192F"); 

                        page.Header().ShowOnce().Row(row =>
                        {
                            var image = Image.FromFile(pathFile);
                            row.ConstantItem(170).Container().Image(image);

                              row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight()
                                    .Background("#64FFDA")
                                    .Padding(10).Border(1).BorderColor("#CCD6F6")
                                    .AlignCenter()
                                    .Text($"Recibo #{invoice.Id}")
                                    .FontColor("#0A192F");

                                col.Item().AlignRight().Text("SMART INTERNATIONAL INVESTMENTS LLC")
                                    .FontColor("#CCD6F6").FontSize(10);
                                col.Item().AlignRight().Text("L24000157429").FontColor("#8892B0").FontSize(10);
                                col.Item().AlignRight()
                                    .Text("Domicilio: España")
                                    .FontColor("#8892B0").FontSize(10);
                                col.Item().AlignRight().Text("Tel/Fax: (561) 694-8107").FontColor("#8892B0")
                                    .FontSize(10);
                                col.Item().AlignRight().Text("Correo: support@thehousecoin.net").FontColor("#8892B0")
                                    .FontSize(10);
                                col.Item().AlignRight().Text(date).FontColor("#8892B0").FontSize(10);
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col1 =>
                        {
                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignLeft().Text("Datos del cliente").Underline().Bold()
                                        .FontColor("#64FFDA");

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Nombre: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span($"{userInfo.Name} {userInfo.LastName}").FontColor("#8892B0")
                                            .FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("País: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Country!.Name).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Ciudad: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.City).FontColor("#8892B0").FontSize(10);
                                    });
                                });

                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Teléfono: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Phone).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("No de Identificación fiscal: ").SemiBold().FontSize(10)
                                            .FontColor("#CCD6F6");
                                        txt.Span(userInfo.UserName).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Correo: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Email).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Domicilio: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                        txt.Span(userInfo.Address).FontColor("#8892B0").FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        if (!string.IsNullOrEmpty(invoice.Reason))
                                        {
                                            txt.Span("Referencia: ").SemiBold().FontSize(10).FontColor("#CCD6F6");
                                            txt.Span(invoice.Reason).FontColor("#8892B0").FontSize(10);
                                        }
                                    });
                                });
                            });

                            col1.Item().LineHorizontal(0.5f).LineColor("#64FFDA");

                            col1.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Concepto")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Cantidad")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Precio")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Descuento")
                                        .FontColor("#0A192F");
                                    header.Cell().Background("#64FFDA").Padding(2).Text("Total")
                                        .FontColor("#0A192F");
                                });

                                foreach (var item in invoice.InvoicesDetails)
                                {
                                    var conceptName = item.ProductName;
                                    var quantity = item.ProductQuantity;
                                    var price = item.ProductPrice;
                                    var tax = item.ProductIva;
                                    var discount = item.ProductDiscount * quantity;
                                    var total = quantity * price;

                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text(conceptName).FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text(quantity.ToString()).FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text($"$ {price:0.##}").FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text($"$ {discount:0.##}").FontColor("#CCD6F6").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#8892B0").Padding(2)
                                        .Text($"$ {total:0.##}").FontColor("#CCD6F6").FontSize(10);

                                    subTotal += total;
                                    totalDiscount += discount;
                                    totalTax = (decimal)tax!;
                                }
                            });

                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text("Monto pagado").Bold().FontSize(16)
                                        .FontColor("#64FFDA");
                                    col2.Item().AlignRight().Text($"Total: ${invoice.TotalInvoice}")
                                        .FontSize(18).FontColor("#64FFDA");
                                    col2.Item().AlignRight().Text("Fecha de pago:").FontSize(12).FontColor("#CCD6F6");
                                    col2.Item().AlignRight().Text(date).FontSize(12).FontColor("#CCD6F6");
                                });
                            });

                            col1.Spacing(10);
                        });

                        page.Footer()
                            .AlignRight()
                            .Text(txt =>
                            {
                                txt.Span("Página ").FontSize(10).FontColor("#8892B0");
                                txt.CurrentPageNumber().FontSize(10).FontColor("#CCD6F6");
                                txt.Span(" de ").FontSize(10).FontColor("#8892B0");
                                txt.TotalPages().FontSize(10).FontColor("#CCD6F6");
                            });
                    });
                });

            byte[] pdfBytes = document.GeneratePdf();
            using var memoryStream = new MemoryStream(pdfBytes);
            return memoryStream.ToArray();
        });
    }
}