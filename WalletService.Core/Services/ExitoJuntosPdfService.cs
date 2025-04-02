using System.Reflection;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.CustomModels;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using Document = QuestPDF.Fluent.Document;

namespace WalletService.Core.Services;

public class ExitoJuntosPdfService : IExitoJuntosPdfService
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
        var pathFile = $"{workingDirectory}{separator}Assets{separator}exito_juntos.png";

        return await Task.Run(() =>
        {
            var document = Document
                .Create(doc =>
                {
                    doc.Page(page =>
                    {
                        page.Margin(15);
                        page.PageColor("#000000");

                        page.Header().ShowOnce().Row(row =>
                        {
                            var image = Image.FromFile(pathFile);
                            row.ConstantItem(170).Container().Image(image);

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight()
                                    .Background("#DAA520")
                                    .Padding(10).Border(1).BorderColor("#DAA520")
                                    .AlignCenter()
                                    .Text($"Recibo #{spResponse.Id}")
                                    .FontColor("#000000");
                                col.Item().AlignRight()
                                    .Text("Domicilio: España")
                                    .FontColor("#DAA520").FontSize(10);
                                col.Item().AlignRight().Text("Correo: support@exitojuntos.com").FontColor("#DAA520")
                                    .FontSize(10);
                                col.Item().AlignRight().Text(date).FontColor("#DAA520").FontSize(10);
                            });
                        });

                        page.Content().PaddingVertical(10).Column(col1 =>
                        {
                            col1.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col2 =>
                                {
                                    col2.Item().AlignLeft().Text("Datos del cliente").Underline().Bold()
                                        .FontColor("#DAA520");

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Nombre: ").SemiBold().FontSize(10).FontColor("#DAA520");
                                        txt.Span($"{userInfo.Name} {userInfo.LastName}").FontColor("#DAA520")
                                            .FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("País: ").SemiBold().FontSize(10).FontColor("#DAA520");
                                        txt.Span(userInfo.Country!.Name).FontColor("#DAA520").FontSize(10);
                                    });

                                    col2.Item().Text(txt =>
                                    {
                                        txt.Span("Ciudad: ").SemiBold().FontSize(10).FontColor("#DAA520");
                                        txt.Span(userInfo.City).FontColor("#DAA520").FontSize(10);
                                    });
                                });
                            });

                            col1.Item().LineHorizontal(0.5f).LineColor("#DAA520");

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
                                    header.Cell().Background("#DAA520").Padding(2).Text("Concepto")
                                        .FontColor("#000000");
                                    header.Cell().Background("#DAA520").Padding(2).Text("Cantidad")
                                        .FontColor("#000000");
                                    header.Cell().Background("#DAA520").Padding(2).Text("Precio")
                                        .FontColor("#000000");
                                    header.Cell().Background("#DAA520").Padding(2).Text("Descuento")
                                        .FontColor("#000000");
                                    header.Cell().Background("#DAA520").Padding(2).Text("Total")
                                        .FontColor("#000000");
                                });

                                foreach (var item in invoice.invoices)
                                {
                                    var conceptName = item.ProductName;
                                    var quantity = item.ProductQuantity;
                                    var price = item.ProductPrice;
                                    var tax = item.ProductIva;
                                    var discount = item.ProductDiscount * quantity;
                                    var total = quantity * price;

                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#DAA520").Padding(2)
                                        .Text(conceptName).FontColor("#DAA520").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#DAA520").Padding(2)
                                        .Text(quantity.ToString()).FontColor("#DAA520").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#DAA520").Padding(2)
                                        .Text($"$ {price:0.##}").FontColor("#DAA520").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#DAA520").Padding(2)
                                        .Text($"$ {discount:0.##}").FontColor("#DAA520").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor("#DAA520").Padding(2)
                                        .Text($"$ {total:0.##}").FontColor("#DAA520").FontSize(10);

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
                                        .FontColor("#DAA520");
                                    col2.Item().AlignRight().Text($"Total: ${invoice.Debit:0.00}")
                                        .FontSize(18).FontColor("#DAA520");
                                    col2.Item().AlignRight().Text("Fecha de pago:").FontSize(12).FontColor("#DAA520");
                                    col2.Item().AlignRight().Text(date).FontSize(12).FontColor("#DAA520");
                                });
                            });
                        });

                        page.Footer()
                            .AlignRight()
                            .Text(txt =>
                            {
                                txt.Span("Página ").FontSize(10).FontColor("#DAA520");
                                txt.CurrentPageNumber().FontSize(10).FontColor("#DAA520");
                                txt.Span(" de ").FontSize(10).FontColor("#DAA520");
                                txt.TotalPages().FontSize(10).FontColor("#DAA520");
                            });
                    });
                });

            byte[] pdfBytes = document.GeneratePdf();
            using var memoryStream = new MemoryStream(pdfBytes);
            return memoryStream.ToArray();
        });
    }
}