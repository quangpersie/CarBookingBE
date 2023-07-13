using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace CarBookingBE.Utils
{
    public class UtilMethods
    {
        public int getSkip(int pageIndex, int limit)
        {
            return (pageIndex - 1) * limit;
        }

        public bool writeToExcel()
        {
            try
            {
                // Danh sách SV
                List<Result<string>> list = new List<Result<string>>()
            {
                new Result<string>(true, "Ket qua 1", "Data 1"),
                new Result<string>(true, "Ket qua 2", "Data 2"),
                new Result<string>(true, "Ket qua 3", "Data 3"),
                new Result<string>(true, "Ket qua 4", "Data 4"),
            };

                // khởi tạo wb rỗng
                XSSFWorkbook wb = new XSSFWorkbook();

                // Tạo ra 1 sheet
                ISheet sheet = wb.CreateSheet();

                // Bắt đầu ghi lên sheet

                // Tạo row
                var row0 = sheet.CreateRow(0);
                // Merge lại row đầu 3 cột
                row0.CreateCell(0); // tạo ra cell trc khi merge
                CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
                sheet.AddMergedRegion(cellMerge);
                row0.GetCell(0).SetCellValue("Test write file Excel");

                // Ghi tên cột ở row 1
                var row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("MSSV");
                row1.CreateCell(1).SetCellValue("Tên");
                row1.CreateCell(2).SetCellValue("Phone");

                // bắt đầu duyệt mảng và ghi tiếp tục
                int rowIndex = 2;
                foreach (var item in list)
                {
                    // tao row mới
                    var newRow = sheet.CreateRow(rowIndex);

                    // set giá trị
                    newRow.CreateCell(0).SetCellValue(item.Success);
                    newRow.CreateCell(1).SetCellValue(item.Message);
                    newRow.CreateCell(2).SetCellValue(item.Data);

                    // tăng index
                    rowIndex++;
                }

                // xong hết thì save file lại
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Excel"), "requestId");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                FileStream fs = new FileStream($"{pathToSave}/result.xlsx", FileMode.CreateNew);
                wb.Write(fs);
                return true;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }
    }
}