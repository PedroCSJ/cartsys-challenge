using Cartsys.Application.DTOs.Developers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Cartsys.Api.Reports;

public static class DeveloperReportService
{
    public static byte[] Generate(IEnumerable<DeveloperDto> developers)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeContent(c, developers));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("Relatório de Desenvolvedores")
                    .FontSize(18).Bold().FontColor(Colors.Blue.Darken3);
                col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });

        container.PaddingTop(10).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);
    }

    private static void ComposeContent(IContainer container, IEnumerable<DeveloperDto> developers)
    {
        var devList = developers.ToList();

        container.PaddingTop(20).Column(col =>
        {
            col.Item().Text($"Total de desenvolvedores: {devList.Count}")
                .FontSize(11).Bold();

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Nome").Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Cidade").Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Estado").Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Senioridade").Bold().FontColor(Colors.White);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                        .Text("Linguagens").Bold().FontColor(Colors.White);
                });

                var isEven = false;
                foreach (var dev in devList)
                {
                    var bg = isEven ? Colors.Grey.Lighten4 : Colors.White;
                    isEven = !isEven;

                    table.Cell().Background(bg).Padding(5).Text(dev.Name);
                    table.Cell().Background(bg).Padding(5).Text(dev.CityName);
                    table.Cell().Background(bg).Padding(5).Text($"{dev.StateName} ({dev.StateUF})");
                    table.Cell().Background(bg).Padding(5).Text(dev.SeniorityLabel);
                    table.Cell().Background(bg).Padding(5)
                        .Text(string.Join(", ", dev.Languages.Select(l => l.Name)));
                }
            });
        });
    }
}
