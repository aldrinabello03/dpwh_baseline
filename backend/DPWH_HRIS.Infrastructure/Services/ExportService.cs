using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using System.Text;
using DPWH_HRIS.Application.Interfaces;

namespace DPWH_HRIS.Infrastructure.Services;

public class ExportService : IExportService
{
    public ExportService()
    {
        // QuestPDF community license for open-source use
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, IEnumerable<string> columns)
    {
        var dataList = data.ToList();
        var columnList = columns.ToList();
        var props = typeof(T).GetProperties().Where(p => columnList.Contains(p.Name)).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(title).SemiBold().FontSize(14).FontColor("#1B2A4A");
                        col.Item().Text($"Generated: {DateTime.Now:MMMM dd, yyyy HH:mm}").FontSize(8).FontColor("#666666");
                    });
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cd =>
                    {
                        foreach (var _ in columnList) cd.RelativeColumn();
                    });

                    // Header row
                    table.Header(header =>
                    {
                        foreach (var col in columnList)
                        {
                            header.Cell().Background("#1B2A4A").Padding(5)
                                .Text(col).FontColor(Colors.White).SemiBold();
                        }
                    });

                    // Data rows
                    bool alternate = false;
                    foreach (var item in dataList)
                    {
                        foreach (var prop in props)
                        {
                            var val = prop.GetValue(item)?.ToString() ?? "";
                            table.Cell().Background(alternate ? "#F5F5F5" : Colors.White).Padding(4).Text(val);
                        }
                        alternate = !alternate;
                    }
                });

                page.Footer().AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        });

        return await Task.FromResult(document.GeneratePdf());
    }

    public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, string? password = null)
    {
        var dataList = data.ToList();
        var props = typeof(T).GetProperties().ToList();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(sheetName);

        // Header row
        for (int i = 0; i < props.Count; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = props[i].Name;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1B2A4A");
            cell.Style.Font.FontColor = XLColor.White;
        }

        // Data rows
        for (int r = 0; r < dataList.Count; r++)
        {
            for (int c = 0; c < props.Count; c++)
            {
                var val = props[c].GetValue(dataList[r]);
                ws.Cell(r + 2, c + 1).Value = val?.ToString() ?? "";
            }
        }

        ws.Columns().AdjustToContents();

        if (!string.IsNullOrEmpty(password))
        {
            ws.Protect(password);
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return await Task.FromResult(ms.ToArray());
    }

    public async Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data)
    {
        var dataList = data.ToList();
        var props = typeof(T).GetProperties().ToList();
        var sb = new StringBuilder();

        // Header
        sb.AppendLine(string.Join(",", props.Select(p => $"\"{p.Name}\"")));

        // Rows
        foreach (var item in dataList)
        {
            sb.AppendLine(string.Join(",", props.Select(p => $"\"{p.GetValue(item)?.ToString()?.Replace("\"", "\"\"") ?? ""}\"")));
        }

        return await Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    public async Task<byte[]> ExportToWordAsync(string content, string title)
    {
        // Simple RTF output as Word placeholder
        // For full Word support, integrate DocumentFormat.OpenXml
        var rtf = $@"{{\rtf1\ansi\deff0
{{\fonttbl{{\f0\fswiss Arial;}}}}
{{\colortbl ;\red27\green42\blue74;}}
\f0\fs28\cf1\b {title}\b0\par\fs20\cf0
{content.Replace("\n", "\\par ")}
}}";
        return await Task.FromResult(Encoding.UTF8.GetBytes(rtf));
    }
}
