using CarBookingBE.Utils;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using CarBookingTest.Models;

namespace CarBookingBE.Services
{
    public class FileService
    {
        MyDbContext _db = new MyDbContext();
        UtilMethods _util = new UtilMethods();
        public Result<string> uploadAvatar(Guid curId, HttpPostedFile postedFile)
        {
            try
            {
                string curUserId = curId.ToString().ToLower();
                string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg" };
                if (postedFile == null || postedFile.FileName.Length == 0)
                {
                    return new Result<string>(false, "Missing file !");
                }
                if (!acceptExtensionImg.Contains(Path.GetExtension(postedFile.FileName)))
                {
                    return new Result<string>(false, "Not support file type ! Please provide image file(.png, .jpg, .jpeg)");
                }
                if (postedFile.ContentLength > (2 * 1024 * 1024))
                {
                    return new Result<string>(false, "The maximum size of file is 20MB !");
                }
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Avatar"), curUserId);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"Files/Avatar/{curUserId}/{postedFile.FileName}");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
        public Result<bool> writeToExcelAndDownload(Guid curId)
        {
            try
            {
                //all requests
                var lRequests = _db.Requests.Where(r => r.IsDeleted == false).ToList();
                if(!lRequests.Any())
                {
                    return new Result<bool>(false, "There's no data !");
                }

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
                row0.GetCell(0).SetCellValue("REPORT OF VEHICLES");

                // Ghi tên cột ở row 1
                var row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("Current date");
                row1.CreateCell(1).SetCellValue("Pick date");
                row1.CreateCell(2).SetCellValue("Pick time");
                row1.CreateCell(3).SetCellValue("Rotation");
                row1.CreateCell(4).SetCellValue("Request ID");
                row1.CreateCell(5).SetCellValue("Status");
                row1.CreateCell(6).SetCellValue("Vehicle type");
                row1.CreateCell(7).SetCellValue("From date");
                row1.CreateCell(8).SetCellValue("To date");
                row1.CreateCell(9).SetCellValue("Usage time from");
                row1.CreateCell(10).SetCellValue("Usage time to");
                row1.CreateCell(11).SetCellValue("Requestor");
                row1.CreateCell(12).SetCellValue("Function");
                row1.CreateCell(13).SetCellValue("Cost center");
                row1.CreateCell(14).SetCellValue("Mobile");
                row1.CreateCell(15).SetCellValue("From location");
                row1.CreateCell(16).SetCellValue("To location");
                row1.CreateCell(17).SetCellValue("Driver");
                row1.CreateCell(18).SetCellValue("Mobile");
                row1.CreateCell(19).SetCellValue("Car plate");
                row1.CreateCell(20).SetCellValue("Note");

                // bắt đầu duyệt mảng và ghi tiếp tục
                int rowIndex = 2;
                foreach (var item in lRequests)
                {
                    // tao row mới
                    var newRow = sheet.CreateRow(rowIndex);
                    var vehicleRequest = _db.VehicleRequests.Where(v => v.IsDeleted == false && v.RequestId == item.Id).FirstOrDefault();

                    if (vehicleRequest == null)
                    {
                        return new Result<bool>(false, "Fetch data fail !");
                    }

                    // set giá trị
                    newRow.CreateCell(0).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.Created));
                    newRow.CreateCell(1).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.Created));
                    newRow.CreateCell(2).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.PickTime));
                    newRow.CreateCell(3).SetCellValue(string.Format("{0:HH:mm}", item.PickTime));
                    newRow.CreateCell(4).SetCellValue(item.RequestCode);
                    newRow.CreateCell(5).SetCellValue(item.Status);
                    newRow.CreateCell(6).SetCellValue(vehicleRequest.Type == true ? "Company vehicle" : "Rented car, taxi");
                    newRow.CreateCell(7).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.UsageFrom));
                    newRow.CreateCell(8).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.UsageTo));
                    newRow.CreateCell(9).SetCellValue(string.Format("{0:HH:mm}", item.UsageFrom));
                    newRow.CreateCell(10).SetCellValue(string.Format("{0:HH:mm}", item.UsageTo));
                    newRow.CreateCell(11).SetCellValue(item.SenderUser.FirstName + " " + item.SenderUser.LastName);
                    newRow.CreateCell(12).SetCellValue(item.SenderUser.Function);
                    newRow.CreateCell(13).SetCellValue(item.CostCenter);
                    newRow.CreateCell(14).SetCellValue(item.Mobile);
                    newRow.CreateCell(15).SetCellValue(item.PickLocation);
                    newRow.CreateCell(16).SetCellValue(item.Destination);
                    newRow.CreateCell(17).SetCellValue(vehicleRequest.User.FirstName + " " + vehicleRequest.User.LastName);
                    newRow.CreateCell(18).SetCellValue(item.Mobile);
                    newRow.CreateCell(19).SetCellValue(vehicleRequest.DriverCarplate);
                    newRow.CreateCell(20).SetCellValue(item.Note);

                    // tăng index
                    rowIndex++;
                }

                // xong hết thì save file lại
                var fileName = "CarBooking_Export_" + DateTime.Now.ToString("ddMMyy");
                Trace.WriteLine(fileName);
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Excel/{curId.ToString().ToLower()}"));
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                //Create # CreateNew
                FileStream fs = new FileStream($"{pathToSave}/{fileName}.xlsx", FileMode.Create);
                wb.Write(fs);
                downloadFileExcel(fileName);
                return new Result<bool>(true, "Write to excel file successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<bool>(false, "Internal error !");
            }
        }

        public HttpResponseMessage downloadFileExcel(string fileName)
        {
            try
            {
                var fileUrl = $"http://localhost:63642/Files/Excel/{fileName}";
                using (WebClient client = new WebClient())
                {
                    byte[] fileData = client.DownloadData(fileUrl);

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileData);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return response;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}