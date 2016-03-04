using System;
using System.IO;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;

namespace Kopiarka.Classes
{
    public abstract class SheetWriter : IDisposable
    {
        public string Filename { get; private set; }
        public string Sheetname { get { return "Spis"; } }
        public string Password { get; private set; }
        public Period FilePeriod { get; private set; }
        public DateTime Date { get; private set; }

        public int Column { get { return 9; } }
        public int Row { get { return 3; } }

        public int Day { get { return FilePeriod == Period.Daily ? Date.Day : 1; } }
        public string Month { get { return String.Format("=DATE(K3,{0},I3)", Date.Month); } }
        public object Year { get { return Date.Year; } }

        public SheetWriter(Period filePeriod, DateTime date, string filename, string password = "")
        {
            FilePeriod = filePeriod;
            Filename = filename;
            Password = password;
            Date = date;
        }

        protected abstract void SetDay();
        protected abstract void SetYear();
        protected abstract void SetMonthFormula();

        public void UpdateDate()
        {
            SetDay();
            SetYear();
            SetMonthFormula();
        }

        public abstract void Dispose();
    }

    public class OfficeSheetWriter : SheetWriter
    {
        private readonly Excel._Workbook workbook;
        private readonly Excel._Worksheet worksheet;

        public OfficeSheetWriter(Excel._Application excel, Period filePeriod, DateTime date, string filename, string password = "")
            : base(filePeriod, date, filename, password)
        {
            workbook = (Excel._Workbook)excel.Workbooks.Open(Filename);
            worksheet = (Excel._Worksheet)workbook.Worksheets[Sheetname];
            worksheet.Unprotect(Password);
        }

        protected override void SetDay()
        {
        	worksheet.Cells[Row, Column] = Day;
        }

        protected override void SetYear()
        {
        	worksheet.Cells[Row, Column + 2] = Year;
        }

        protected override void SetMonthFormula()
        {
        	worksheet.Cells[Row, Column + 1] = Month;
        }

        public override void Dispose()
        {
        	worksheet.Protect(Password);
        	workbook.Save();
        	workbook.Close();
        }
        
        public static Excel._Application CreateExcel()
        {
            return new Excel.Application
            {
                Visible = false,
                AskToUpdateLinks = false,
                DisplayAlerts = false
            };
        }
    }

    public class EppSheetWriter : SheetWriter
    {
        private readonly ExcelPackage excel;
        private readonly ExcelWorksheet worksheet;

        public EppSheetWriter(Period filePeriod, DateTime date, string filename, string password = "")
            : base(filePeriod, date, filename, password)
        {
            excel = new ExcelPackage(new FileInfo(Filename));
            worksheet = excel.Workbook.Worksheets[Sheetname];
            worksheet.Protection.IsProtected = false;
        }

        protected override void SetDay()
        {
            worksheet.Cells[Row, Column].Value = Day;  
        }

        protected override void SetYear()
        {
            worksheet.Cells[Row, Column + 2].Value = Year;
        }

        protected override void SetMonthFormula()
        {
            worksheet.Cells[Row, Column + 1].Formula = Month;
        }

        public override void Dispose()
        {
            worksheet.Protection.SetPassword(Password);
            worksheet.Protection.IsProtected = true;
            excel.Save();
            excel.Dispose();
        }
    }
}
