using CarBookingBE.Utils;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using CarBookingTest.Models;
using QRCoder;
using System.Drawing;
using IronPdf;

namespace CarBookingBE.Services
{
    public class FileService
    {
        MyDbContext _db = new MyDbContext();
        UtilMethods _util = new UtilMethods();
        public Result<string> uploadAvatar(string currentId, HttpPostedFile postedFile)
        {
            try
            {
                var curId = Guid.Parse(currentId);
                string curUserId = curId.ToString().ToLower();
                string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg" };
                if(currentId == null)
                {
                    return new Result<string>(false, "Missing user id of the avatar !");
                }
                if (postedFile == null || postedFile.FileName.Length == 0)
                {
                    return new Result<string>(false, "Missing file !", currentId);
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

        public Result<string> copyAvatarFromTemp(string userId, string fileName)
        {
            try
            {
                if(userId == null || fileName == null)
                {
                    return new Result<string>(false, "Missing parameter(s) !");
                }
                //need initial
                var userAvararFolder = HttpContext.Current.Server.MapPath($"~/Files/Avatar/{userId}");
                var destPath = $"{userAvararFolder}/{fileName}";
                if (!Directory.Exists(userAvararFolder))
                {
                    Directory.CreateDirectory(userAvararFolder);
                }
                var tempPath = HttpContext.Current.Server.MapPath($"~/Files/Avatar/temp/{fileName}");

                if (File.Exists(tempPath))
                {
                    if(!File.Exists(destPath))
                    {
                        File.Copy(tempPath, destPath);
                    }
                    File.Delete(tempPath);
                    return new Result<string>(true, "Finish uploading avatar !", $"Files/Avatar/{userId}/{fileName}");
                }
                return new Result<string>(false, "File by path does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
        public Result<string> uploadAvatarTemp(HttpPostedFile postedFile)
        {
            try
            {
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
                string pathToSave = HttpContext.Current.Server.MapPath($"~/Files/Avatar/temp");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"{pathToSave}/{postedFile.FileName}");
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

        public Result<string> createQRCode(string link)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);

            // Create the QR code
            QRCode qrCode = new QRCode(qrCodeData);

            // Convert the QR code to a bitmap image
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/QRCode"));
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            // Save the QR code image to a local file
            string imagePath = $"{pathToSave}/qrcode.png";
            qrCodeImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            return new Result<string>(false, "QR code image saved to: " + imagePath);
        }

        public Result<string> writeRequestToPdf()
        {
            try
            {
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Pdf"));
                string pdfFilePath = Path.Combine(pathToSave, "test.pdf");
                string htmlFilePath = Path.Combine(pathToSave, "minify.html");

                var result = createHtmlFromRequest(new Request(), htmlFilePath);
                if (!result) return new Result<string>(false, "Html content error !");
                //string htmlContent = File.ReadAllText(htmlFilePath);

                // Create a PDF renderer
                var renderer = new ChromePdfRenderer();

                // Convert the HTML file to PDF
                var pdfDocument = renderer.RenderHtmlFileAsPdf(htmlFilePath);

                // Save the PDF to a file
                pdfDocument.SaveAs(pdfFilePath);

                return new Result<string>(true, "write pdf ok !", pdfFilePath);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "write pdf fail !");
            }
        }

        public bool createHtmlFromRequest(Request request, string htmlFilePath)
        {
            try
            {
                string htmlContent = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <style>.title{font-weight:700}.title-cbr{font-weight:700;font-size:20px}.m-top{margin-top:20px}.p-top{padding-top:10px}.text-center{text-align:center}.text-left{text-align:left}.flex-row{display:flex}.flex-col{display:flex;flex-direction:column}.align-center{align-items:center}.gap-12{gap:12px}.gap-36{gap:36px}.line{height:1.2px;width:60%;background-color:#000}.full-line{height:2px;background-color:#000}.full-line-grey{height:1.2px;background-color:grey}.col-3{flex:1}.col-4{flex:2}.p-16{padding:0 16px}.pt-8{padding-top:8px}.note{font-size:14px;margin-top:20px;text-align:justify}.label-radio{font-size:18px}.height-30{height:30px}.min-width{min-width:152px}.radio{border-radius:50%;border:1px solid black;height:14px;width:14px;display:flex;justify-content:center;align-items:center;}.radio-check{border-radius:50%;border:1px solid black;height:6px;width:6px;background-color:black;}.gap-8{gap:4px;}</style>
                <body>
                    <div>
                        <img src=""https://drive.google.com/uc?export=view&id=1-ib3rZw6Dq_oaBMyJarHckgEfaIOmYjf"" alt=""qrCodeImage"">
                    </div>
                    <div class=""flex-row"" style=""justify-content: space-around;"">
                        <div class=""flex-col align-center gap-12"">
                            <span class=""title"">OPUS SOLUTION COMPANY</span>
                            <div class=""line""></div>
                            <span>No: 2023OPS-CAR-0710-004</span>
                        </div>
                        <div class=""flex-col align-center gap-12"">
                            <span class=""title"">Status: Approved</span>
                            <div class=""line""></div>
                            <span>10/07/2023 15:48 PM</span>
                        </div>
                    </div>

                    <div class=""p-16"">
                        <div class=""title-cbr m-top text-center"">CAR BOOKING REQUEST</div>
                        <div class=""flex-col gap-12 m-top"">
                            <div class=""flex-row"">
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Applicant</div>
                                    <span>Bang Nguyen Minh</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Department</div>
                                    <span>IT/ Technical</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">User</div>
                                    <span>Bang Nguyen Minh</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Mobile</div>
                                    <span>0876839834</span>
                                </div>
                            </div>
                            <div class=""flex-row"">
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Cost Center</div>
                                    <span>12</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Total Passengers</div>
                                    <span>2</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Usage time from</div>
                                    <span>20/07/2023 11:29 AM - 20/07/2023 12:29 PM</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Pick time</div>
                                    <span>20/07/2023 11:29 AM</span>
                                </div>
                            </div>
                            <div class=""flex-row"">
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Pick Location</div>
                                    <span>Ho Chi Minh</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Destination</div>
                                    <span>Ha Noi</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Reason</div>
                                    <span>Delay</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title""></div>
                                    <span></span>
                                </div>
                            </div>
                        </div>
                        <p class='note'>Note: In case the Administration Department does not have enough vehicles to meet the department's vehicle dispatching requirements, the Department The Administration proposes to arrange alternative means of transportation (hire a car, or use a taxi card, Grab,...) and Costs will be accounted for according to the required department.</p>
                        <div class=""flex-row gap-12"">
                            <div class=""flex-row align-center height-30 gap-8"">
                                <div class=""radio"">
                                    <div class=""radio-check""></div>
                                </div>
                                <label for=""yes"" class=""label-radio"">Yes</label>
                            </div>
                            <div class=""flex-row align-center height-30 gap-8"">
                                <div class=""radio"">
                                    <div class=""""></div>
                                </div>
                                <label for=""no"" class=""label-radio"">No</label>
                            </div>
                        </div>

                        <!-- document signer -->
                        <div class=""title-cbr text-left m-top"">Document Signers</div>
                        <div class=""m-top"">
                            <div class=""full-line m-top""></div>
                            <div class=""flex-row align-center gap-12"">
                                <div class=""col-3"">
                                    <div>
                                        <div class=""flex-row pt-8"">
                                            <div class=""title min-width"">Title</div>
                                            <div>Developer</div>
                                        </div>
                                    </div>
                                    <div class=""p-top"">
                                        <div class=""full-line-grey""></div>
                                        <div class=""flex-row pt-8"">
                                            <div class=""title min-width"">Email</div>
                                            <div>bangnm@o365.vn</div>
                                        </div>
                                    </div>
                                    <div class=""p-top"">
                                        <div class=""full-line-grey""></div>
                                        <div class=""flex-row pt-8"">
                                            <div class=""title min-width"">Status</div>
                                            <div>APPROVED at Mon, 10 Jul 2023 15:49:05 +07:00</div>
                                        </div>
                                    </div>
                                </div>
                                <div style=""padding: 8px 8px 0 8px;"">
                                    <img src=""https://drive.google.com/uc?export=view&id=1IKjLsNESdLfgbYFTDROq599dQd58ILlO"" alt=""qrCodeImage"" width=""120"" height=""120"">
                                </div>
                            </div>
                            <div class=""p-top"">
                                <div class=""full-line-grey""></div>
                                <div class=""flex-row pt-8"">
                                    <div class=""title min-width"">Name</div>
                                    <div>Bang Nguyen Minh</div>
                                </div>
                            </div>
                        </div>

                        <!-- related document -->
                        <div>
                            <div class=""title-cbr text-left m-top"">Related document</div>
                            <div class=""full-line m-top""></div>

                            <div class=""flex-col m-top"">
                                <div class=""flex-row gap-36"">
                                    <div class=""col-3"">Thu, 20 Jul 2023 11:27:25 +07:00</div>
                                    <a class=""col-4"" href=""https://www.youtube.com"">Car Booking API.postman_collection.json</a>
                                    <span class=""col-3"">Bang Nguyen Minh</span>
                                </div>
                                <div class=""flex-row gap-36"">
                                    <div class=""col-3"">Thu, 20 Jul 2023 11:27:25 +07:00</div>
                                    <a class=""col-4"" href=""https://www.youtube.com"">Car Booking API.postman_collection.json</a>
                                    <span class=""col-3"">Bang Nguyen Minh</span>
                                </div>
                            </div>
                        </div>

                        <div>
                            <div class=""title text-left m-top"">Discussion log</div>
                            <div class=""full-line m-top""></div>

                            <div class=""flex-col m-top"">
                                <div class=""flex-row gap-36"">
                                    <div class=""col-3"">Thu, 20 Jul 2023 11:27:25 +07:00</div>
                                    <div class=""col-4"">Submit the request 2023OPS-CAR-0720-001 for approval</div>
                                    <span class=""col-3"">Bang Nguyen Minh</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";
                File.WriteAllText(htmlFilePath, htmlContent);
                return true;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public Result<string> deleteAllFilesInFolder(string filePath)
        {
            try
            {
                if(filePath == null || filePath.Length == 0)
                {
                    return new Result<string>(false, "File path is empty !");
                }
                string pathToDel = HttpContext.Current.Server.MapPath($"~/{filePath}");
                if (Directory.Exists(pathToDel))
                {
                    string[] files = Directory.GetFiles(pathToDel);

                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }

                    return new Result<string>(true, "Delete files successfully !");
                }
                return new Result<string>(false, "File path does not exist !");
            }
            catch(Exception e )
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
    }
}