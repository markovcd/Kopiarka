using System;
using System.IO;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;

namespace Kopiarka.Classes
{
    public class SheetWriter : IDisposable
    {
        private readonly ExcelPackage excel;
        private readonly ExcelWorksheet worksheet;
        private readonly string password;

        public SheetWriter(string filename, string sheet, string password = "")
        {
            excel = new ExcelPackage(new FileInfo(filename));
            worksheet = excel.Workbook.Worksheets[sheet];
            this.password = password;
            worksheet.Protection.IsProtected = false;
        }

        public void UpdateDate(DateTime date, Period filePeriod)
        {
            worksheet.Cells[3, 9].Value = filePeriod == Period.Daily ? date.Day.ToString() : "1";
            worksheet.Cells[3, 10].Formula = String.Format("=DATE(K3,{0},I3)", date.Month);
            worksheet.Cells[3, 11].Value = date.Year;
        }

        public static void UpdateDate(DateTime date, Period filePeriod, string filename,
            string password = "")
        {
            using (var sheet = new SheetWriter(filename, "Spis", password))
                sheet.UpdateDate(date, filePeriod);
        }

        public void Dispose()
        {
            worksheet.Protection.SetPassword(password);
            worksheet.Protection.IsProtected = true;
            excel.Save();
            excel.Dispose();
        }
    }
}
