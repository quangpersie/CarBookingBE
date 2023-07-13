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
    public class RequestAttachmentService
    {
        private MyDbContext db = new MyDbContext();
        public Result<List<RequestAttachmentDTO>> GetAttachmentByRequestId (string id)
        {
            List <RequestAttachmentDTO> requestAttachmentList = db.RequestAttachments.Where(ra => ra.RequestId.ToString() == id)
                 .Select(ra => new RequestAttachmentDTO() {
                     Id = ra.Id,
                     Path = ra.Path
                 })
                 .ToList();
            return new Result<List<RequestAttachmentDTO>>(true, "Get Request Attachments Success", requestAttachmentList);
        }

        public Result<RequestAttachment> CreateAttachment (HttpPostedFile file, string requestId)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id.ToString() == requestId && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<RequestAttachment>(false, "Request Not Exist");
            }
            /*if (files.Length == 0)
            {
                return new Result<List<RequestAttachment>>(false, "Attachment is required");
            }
            foreach (HttpPostedFileBase file in files)*/
            if (file != null)
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
            }

            return new Result<RequestAttachment>(true, "Create Attachments Success");
        }

        public Result<string> uploadFile(HttpPostedFile postedFile, string requestCode, string requestId)
        {
            string newPath = "Files/Attachments/" + requestCode + "/" + postedFile.FileName;
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
            string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Attachments"), requestCode);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
            return new Result<string>(true, "Upload file successfully !", $"Files/Attachments/{requestCode}/{postedFile.FileName}");
        }
    }
}