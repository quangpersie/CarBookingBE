using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CarBookingBE.Services
{
    public class RequestCommentService
    {
        MyDbContext db = new MyDbContext();

        public Result<List<RequestCommentDTO>> GetAllCommentByRequestId(string requestId)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id.ToString() == requestId && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<List<RequestCommentDTO>>(false, "Request Not Found");
            }

            List<RequestCommentDTO> requestComments =
                db.RequestComments.Where(rc => rc.RequestId.ToString() == requestId)
                .Select(rc => new RequestCommentDTO()
                {
                    Id = rc.Id,
                    Account = new AccountDTO()
                    {
                        Id = rc.Account.Id,
                        FirstName = rc.Account.FirstName,
                        LastName = rc.Account.LastName
                    },
                    Content = rc.Content,
                    Created = rc.Created
                })
                .ToList();

            return new Result<List<RequestCommentDTO>>(true, "Get Success", requestComments);
        }

        public Result<RequestComment> CreateComment (HttpPostedFile file, RequestComment requestComment, string requestId)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id.ToString() == requestId && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<RequestComment>(false, "Request Not Exist");
            }
            if (file == null)
            {
                db.RequestComments.Add(requestComment);
                db.SaveChanges();
            }
            /*if (file != null)
            {

                RequestAttachment requestAttachment = new RequestAttachment();
                requestAttachment.IsDeleted = false;
                requestAttachment.RequestId = Guid.Parse(requestId);
                var rs = uploadFile(file, request.RequestCode, requestId);
                if (!rs.Success)
                {
                    return new Result<RequestAttachment>(false, rs.Message);
                }
                requestAttachment.Path = rs.Data;
                db.RequestAttachments.Add(requestAttachment);
                db.SaveChanges();
            }*/

            return new Result<RequestComment>(true, "Create Success");
        }


        //-----------------Function----------------------------
        protected Result<string> uploadFile(HttpPostedFile postedFile, string requestCode, string requestId)
        {
            string newPath = "Files/Comments/" + requestCode + "/" + postedFile.FileName;
            List<RequestAttachment> requestAttachments = db.RequestAttachments.ToList();
            foreach (RequestAttachment req in requestAttachments)
            {
                if (req.Path == newPath && req.RequestId.ToString() == requestId)
                {
                    return new Result<string>(false, "File: " + postedFile.FileName + " is exist in this Request");
                }
            }
            string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg", ".pdf", ".csv", ".doc", ".docx", ".pptx", ".ppt", ".txt", "xls", "xlsx" };
            if (postedFile == null || postedFile.FileName.Length == 0)
            {
                return new Result<string>(false, "Missing file !");
            }
            if (!acceptExtensionImg.Contains(Path.GetExtension(postedFile.FileName)))
            {
                return new Result<string>(false, "Not support file type !");
            }
            if (postedFile.ContentLength > (2 * 1024 * 1024))
            {
                return new Result<string>(false, "The maximum size of file is 20MB !");
            }
            string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Comments"), requestCode);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
            return new Result<string>(true, "Upload file successfully !", $"Files/Comments/{requestCode}/{postedFile.FileName}");
        }
    }
}