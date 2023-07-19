using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using CarBookingBE.DTOs;
using CarBookingTest.Models;
using System.IdentityModel.Tokens.Jwt;

namespace CarBookingBE.Utils
{
    public class UtilMethods
    {
        MyDbContext _db = new MyDbContext();
        public Result<Guid> getCurId()
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                HttpContext httpContext = HttpContext.Current;
                var jwt = httpContext.Request.Headers["Authorization"];
                Trace.WriteLine(jwt);
                var jwtTokenObj = tokenHandler.ReadJwtToken(jwt.Substring(7)); //ignore "Bearer "
                string curId = "";
                foreach (var claim in jwtTokenObj.Claims)
                {
                    //Trace.WriteLine($"{claim.Type}: {claim.Value}");
                    if (claim.Type.Equals("CurId"))
                    {
                        curId = claim.Value;
                        break;
                    }
                }
                return new Result<Guid>(true, "Get curId successfully !", Guid.Parse(curId));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Guid>(false, "Internal error !");
            }
        }
        public bool isAuthorized(List<string> requiredRoles)
        {
            try
            {
                var curIdObj = getCurId();
                if(curIdObj.Success == false)
                {
                    return false;
                }
                var userRoleList = userRoles(curIdObj.Data);
                if (userRoleList.Success == false) { return false; }
                if (userRoleList.Data.Any())
                {
                    foreach (var item in requiredRoles)
                    {
                        if (userRoleList.Data.Contains(item)) { return true; }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public Result<List<string>> userRoles(Guid curId)
        {
            try
            {
                var uRoles = new List<string>();
                var getUserRoles = _db.UserRoles
                    .Where(u => u.IsDeleted == false && u.UserId == curId)
                    .Select(u => new UserRolesDTO
                    {
                        Role = new RoleDTO
                        {
                            Id = u.Role.Id,
                            Title = u.Role.Title
                        }
                    })
                    .ToList();
                if (!getUserRoles.Any())
                {
                    return new Result<List<string>>(false, "User does not have any roles !");
                }
                foreach (var item in getUserRoles)
                {
                    uRoles.Add(item.Role.Title);
                }
                return new Result<List<string>>(true, "Get user roles successfully !", uRoles);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<string>>(false, "Internal error !");
            }
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
                //Create # CreateNew
                FileStream fs = new FileStream($"{pathToSave}/result.xlsx", FileMode.Create);
                wb.Write(fs);
                return true;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public HttpResponseMessage downloadFile()
        {
            try
            {
                var fileName = "result.xlsx";
                var requestId = "requestId";
                var fileUrl = $"http://localhost:63642/Files/Excel/{requestId}/{fileName}";
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
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}