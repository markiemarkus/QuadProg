namespace QuadProg.Setup
{
    using Microsoft.Office.Interop.Excel;
    using System;
    using ExcelApp = Microsoft.Office.Interop.Excel.Application;

    public class ExcelSpreadSheet
    {
        public Workbook GetExcelWorkBook(string filename)
        {
            ExcelApp app = null;

            try
            {
                app = (ExcelApp)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch
            {
                app = new ExcelApp();
            }

            app.Visible = true;

            Workbook book = app.Workbooks.Open(filename);

            return book;
        }
        
        public double[,] ReadRange(Workbook book, string worksheetName, string rangeAddress)
        {
            Worksheet sheet = (Worksheet)book.Sheets[worksheetName];

            Range range = sheet.get_Range(rangeAddress);

            object[,] src = (object[,])range.Value2;

            double[,] values = new double[src.GetLength(0), src.GetLength(1)];
            Array.Copy(src, values, src.Length);

            return values;
        }
    }
}
