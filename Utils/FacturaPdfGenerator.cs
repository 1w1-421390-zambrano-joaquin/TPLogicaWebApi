namespace TPLogicaWebApi.Utils
{
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using TPLogicaWebApi.DATA.DTOs.FacturasDTOs;

    public static class FacturaPdfGenerator
    {
        public static byte[] Generar(FacturaPdfDto f)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));
                    page.PageColor(Colors.White);

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Mi Farmacia")
                                    .FontSize(20)
                                    .Bold();

                                c.Item().Text($"Factura N° {f.IdFactura}")
                                    .FontSize(11);

                                c.Item().Text($"Tipo: {f.TipoFactura}")
                                    .FontSize(11);

                                c.Item().Text($"Fecha: {f.Fecha:dd/MM/yyyy}")
                                    .FontSize(11);
                            });

                            row.ConstantItem(220).Column(c =>
                            {
                                c.Item().Text("Cliente:")
                                    .Bold();
                                c.Item().Text(f.ClienteNombre);

                                c.Item().Text("Atendido por:")
                                    .Bold();
                                c.Item().Text(f.EmpleadoNombre);
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(8);
                        col.Item().Text("Detalle de la factura")
                            .Bold()
                            .FontSize(12);

                        col.Item().Table(table =>
                        {
                            //columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(6);  // Producto
                                columns.RelativeColumn(1.5f); // Cant.
                                columns.RelativeColumn(2);  // P. Unit
                                columns.RelativeColumn(2);  // Subtotal
                            });

                            
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Producto");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Cant.");
                                header.Cell().Element(HeaderCell).AlignRight().Text("P. Unit.");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Subtotal");
                            });

                            decimal total = 0;
                            var detalles = f.Detalles ?? new List<FacturaPdfDetalleDto>();

                            for (int i = 0; i < detalles.Count; i++)
                            {
                                var d = detalles[i];
                                var sub = d.PrecioUnitario * d.Cantidad;
                                total += sub;

                                bool esPar = i % 2 == 0;

                                table.Cell().Element(c => BodyCell(c, esPar))
                                    .Text(d.ProductoNombre);

                                table.Cell().Element(c => BodyCell(c, esPar))
                                    .AlignRight()
                                    .Text(d.Cantidad.ToString());

                                table.Cell().Element(c => BodyCell(c, esPar))
                                    .AlignRight()
                                    .Text(d.PrecioUnitario.ToString("C"));

                                table.Cell().Element(c => BodyCell(c, esPar))
                                    .AlignRight()
                                    .Text(sub.ToString("C"));
                            }

                            // Fila total
                            table.Cell().ColumnSpan(3)
                                .Element(c => c.PaddingTop(5))
                                .AlignRight()
                                .Text("TOTAL:")
                                .Bold()
                                .FontSize(11);

                            table.Cell()
                                .Element(c => c.PaddingTop(5))
                                .AlignRight()
                                .Text(total.ToString("C"))
                                .Bold()
                                .FontSize(11);
                        });

                        // Notas o footer interno opcional
                        col.Item().PaddingTop(15).Text("Gracias por su compra.")
                            .Italic()
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    page.Footer()
    .AlignCenter()
    // ACA FLACO!! -> el estilo del texto del footer se configura acá
    .DefaultTextStyle(x => x.FontSize(8))
    .Text(txt =>
    {
        txt.Span("Mi Farmacia - ");
        txt.Span("Emitido automáticamente")
           .FontColor(Colors.Grey.Darken1);
    });

                });
            });

            return doc.GeneratePdf();
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container
                .PaddingVertical(4)
                .PaddingHorizontal(3)
                .Background(Colors.Grey.Lighten3)
                .DefaultTextStyle(x => x.Bold().FontSize(10));
        }
        private static IContainer BodyCell(IContainer container, bool esPar)
        {
            return container
                .PaddingVertical(3)
                .PaddingHorizontal(3)
                .Background(esPar ? Colors.Grey.Lighten5 : Colors.White);
        }
    }


}
