using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;

namespace Application.Helpers;

public static class ExportExcelFileHelper
{
    public static byte[] ExportToExcelAsync<T>(List<T> data, string sheetName = "Sheet1")
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (string.IsNullOrWhiteSpace(sheetName)) sheetName = "Sheet1";

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        var properties = typeof(T).GetProperties();
        for (var i = 0; i < properties.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = properties[i].Name;
        }

        for (var i = 0; i < data.Count; i++)
        {
            var item = data[i];
            for (var j = 0; j < properties.Length; j++)
            {
                var value = properties[j].GetValue(item, null);
                if (value is DateTime dateTime)
                {
                    worksheet.Cell(i + 2, j + 1).Value = dateTime.ToString("dd/MM/yyyy");
                    continue;
                }
                worksheet.Cell(i + 2, j + 1).Value = value?.ToString();
            }
        }

        var headerRange = worksheet.Range(1, 1, 1, properties.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }
}