using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Reflection;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services;

public class MediatorPdfService : IMediatorPdfService
{

    public async Task<byte[]> GenerateInvoice(UserInfoResponse userInfo, DebitTransactionRequest invoice, InvoicesSpResponse spResponse)
    {
        var date             = DateTime.Now.ToString("MM/dd/yyyy");
        var totalTax         = 0m;
        var subTotal         = 0m;
        var totalDiscount    = 0m;
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;
        var pathFile         = $"{workingDirectory}{separator}Assets{separator}logo.png";

        return await Task.Run(() =>
        {
            var document = Document
                .Create(doc =>
                {
                    doc.Page(page =>
                    {
                        page.Margin(15);

                        page.Header().ShowOnce().Row(row =>
                        {
                            var image = Image.FromFile(pathFile);
                            row.ConstantItem(180).Container().Image(image);

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight().Background("#257272").Border(1).BorderColor("#257272").AlignCenter()
                                    .Text($"Factura de servicios #{spResponse.Id}")
                                    .FontColor("#fff");

                                col.Item().AlignRight().Text("Razón Social: Ecosystem OCX Sharing Evolution S.A.").FontSize(10);
                                col.Item().AlignRight().Text("C.I.F: 3-101-844938").FontSize(10);
                                col.Item().AlignRight().Text("Domicilio: San José-Santa Ana, Brasil, Centro Comercial Plaza del Rio, Local #3").FontSize(10);
                                col.Item().AlignRight().Text("Tel/Fax: 50663321239").FontSize(10);
                                col.Item().AlignRight().Text("Correo: facturacion@ecosystemfx.com").FontSize(10);
                                col.Item().AlignRight().Text(date).FontSize(10);
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col1 =>
                        {
                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignLeft().Text("Datos del cliente").Underline().Bold();

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Nombre: ").SemiBold().FontSize(10);
                                        txt.Span($"{userInfo.Name} {userInfo.LastName}").FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("País: ").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Country!.Name).FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Ciudad: ").SemiBold().FontSize(10);
                                        txt.Span(userInfo.City).FontSize(10);
                                    });
                                });

                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Teléfono:").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Phone).FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("No de Identificación fiscal:").SemiBold().FontSize(10);
                                        txt.Span(userInfo.UserName).FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Correo:").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Email).FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Domicilio: ").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Address).FontSize(10);
                                    });
                                });
                            });


                            col1.Item().LineHorizontal(0.5f);

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
                                    header.Cell().Background("#257272").Padding(2).Text("Concepto").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Cantidad").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Precio").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Descuento").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Total").FontColor("#fff");
                                });

                                foreach (var item in invoice.invoices)
                                {
                                    var conceptName = item.ProductName;
                                    var quantity    = item.ProductQuantity;
                                    var price       = item.ProductPrice; 
                                    var tax         = item.ProductIva;
                                    var discount    = item.ProductDiscount * quantity;
                                    var total       = quantity * price; 

                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(conceptName).FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(quantity.ToString()).FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text($"$ {price.ToString("0.##")}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text($"$ {discount.ToString("0.##")}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2)
                                        .Text($"$ {total.ToString("0.##")}").FontSize(10);

                                    subTotal      += total;
                                    totalDiscount += discount;
                                    totalTax      =  tax;
                                }
                            });

                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text("Monto pagado").Bold().FontSize(16);
                                    col2.Item().AlignRight().Text($"Total: ${invoice.Debit.ToString("0.00")}").FontSize(18).FontColor("30A2FF");
                                    col2.Item().AlignRight().Text("Fecha de pago:").FontSize(12);
                                    col2.Item().AlignRight().Text(date).FontSize(12);
                                });
                            });

                            col1.Spacing(10);
                        });

                        page.Footer()
                            .AlignRight()
                            .Text(txt =>
                            {
                                txt.Span("Página ").FontSize(10);
                                txt.CurrentPageNumber().FontSize(10);
                                txt.Span(" de ").FontSize(10);
                                txt.TotalPages().FontSize(10);
                            });
                    });
                });

            byte[] pdfBytes = document.GeneratePdf();

            using var memoryStream = new MemoryStream(pdfBytes);
            return memoryStream.ToArray();
        });
    }

        public async Task<byte[]> RegenerateInvoice(UserInfoResponse userInfo, Invoices invoice)
    {
        var date             = DateTime.Now.ToString("MM/dd/yyyy");
        var totalTax         = 0m;
        var subTotal         = 0m;
        var totalDiscount    = 0m;
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;
        var pathFile         = $"{workingDirectory}{separator}Assets{separator}logo.png";

        return await Task.Run(() =>
        {
            var document = Document
                .Create(doc =>
                {
                    doc.Page(page =>
                    {
                        page.Margin(15);

                        page.Header().ShowOnce().Row(row =>
                        {
                            var image = Image.FromFile(pathFile);
                            row.ConstantItem(180).Container().Image(image);

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight().Background("#257272").Border(1).BorderColor("#257272").AlignCenter()
                                    .Text($"Factura de servicios #{invoice.Id}")
                                    .FontColor("#fff");

                                col.Item().AlignRight().Text("Razón Social: Ecosystem OCX Sharing Evolution S.A.").FontSize(10);
                                col.Item().AlignRight().Text("C.I.F: 3-101-844938").FontSize(10);
                                col.Item().AlignRight().Text("Domicilio: San José-Santa Ana, Brasil, Centro Comercial Plaza del Rio, Local #3").FontSize(10);
                                col.Item().AlignRight().Text("Tel/Fax: 50663321239").FontSize(10);
                                col.Item().AlignRight().Text("Correo: facturacion@ecosystemfx.com").FontSize(10);
                                col.Item().AlignRight().Text(date).FontSize(10);
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col1 =>
                        {
                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignLeft().Text("Datos del cliente").Underline().Bold();

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Nombre: ").SemiBold().FontSize(10);
                                        txt.Span($"{userInfo.Name} {userInfo.LastName}").FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("País: ").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Country!.Name).FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Ciudad: ").SemiBold().FontSize(10);
                                        txt.Span(userInfo.City).FontSize(10);
                                    });
                                });

                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Teléfono:").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Phone).FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("No de Identificación fiscal:").SemiBold().FontSize(10);
                                        txt.Span(userInfo.UserName).FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Correo:").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Email).FontSize(10);
                                    });

                                    col2.Item().AlignRight().Text(txt =>
                                    {
                                        txt.Span("Domicilio: ").SemiBold().FontSize(10);
                                        txt.Span(userInfo.Address).FontSize(10);
                                    });
                                });
                            });


                            col1.Item().LineHorizontal(0.5f);

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
                                    header.Cell().Background("#257272").Padding(2).Text("Concepto").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Cantidad").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Precio").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Descuento").FontColor("#fff");
                                    header.Cell().Background("#257272").Padding(2).Text("Total").FontColor("#fff");
                                });

                                foreach (var item in invoice.InvoiceDetail)
                                {
                                    var conceptName = item.ProductName;
                                    var quantity    = item.ProductQuantity;
                                    var price       = item.ProductPrice; 
                                    var tax         = item.ProductIva;
                                    var discount    = item.ProductDiscount * quantity;
                                    var total       = quantity * price; 

                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(conceptName).FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text(quantity.ToString()).FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text($"$ {price.ToString("0.##")}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2).Text($"$ {discount.ToString("0.##")}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9").Padding(2)
                                        .Text($"$ {total.ToString("0.##")}").FontSize(10);

                                    subTotal      += total;
                                    totalDiscount += discount;
                                    totalTax      =  (decimal)tax;
                                }
                            });

                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignRight().Text("Monto pagado").Bold().FontSize(16);
                                    col2.Item().AlignRight().Text($"Total: ${invoice.TotalInvoice.ToString()}").FontSize(18).FontColor("30A2FF");
                                    col2.Item().AlignRight().Text("Fecha de pago:").FontSize(12);
                                    col2.Item().AlignRight().Text(date).FontSize(12);
                                });
                            });

                            col1.Spacing(10);
                        });

                        page.Footer()
                            .AlignRight()
                            .Text(txt =>
                            {
                                txt.Span("Página ").FontSize(10);
                                txt.CurrentPageNumber().FontSize(10);
                                txt.Span(" de ").FontSize(10);
                                txt.TotalPages().FontSize(10);
                            });
                    });
                });

            byte[] pdfBytes = document.GeneratePdf();

            using var memoryStream = new MemoryStream(pdfBytes);
            return memoryStream.ToArray();
        });
    }
}