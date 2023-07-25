using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingBE.Utils;
using CarBookingTest.Utils;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        FileService fileService = new FileService();
        RoleConstants roleConstants = new RoleConstants();
        UtilMethods util = new UtilMethods();

        [HttpGet]
        [Route("cd-xlxs-request")]
        [JwtAuthorize]
        public HttpResponseMessage writeExcel()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, true, true, true, true));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            var curId = isAuthorized.Data;
            return Request.CreateResponse(HttpStatusCode.OK, fileService.writeToExcelAndDownload(curId));
        }

        [HttpGet]
        [Route("c-png-qrcode")]
        public HttpResponseMessage createQRCode()
        {
            string link = "http://localhost:3000";
            return Request.CreateResponse(HttpStatusCode.OK, fileService.createQRCode(link));
        }

        [HttpGet]
        [Route("pdf-request")]
        public HttpResponseMessage createPdfRequest()
        {
            return Request.CreateResponse(HttpStatusCode.OK, fileService.writeRequestToPdf());
        }

        [HttpGet]
        [Route("delete-files-temp")]
        public IHttpActionResult deleteFile()
        {
            var filePath = "Files/Avatar/temp";
            return Ok(fileService.deleteAllFilesInFolder(filePath));
        }

        /*[HttpGet]
        [Route("download")]
        [JwtAuthorize]
        public HttpResponseMessage downloadFileExcel()
        {
            roleConstants = new RoleConstants(true, true, true, true, true);
            var isAuthorized = util.isAuthorized(roleConstants.Roles);
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            //var curId = isAuthorized.Data;
            return fileService.downloadFileExcel();
        }*/

        [HttpPost]
        [Route("upload")]
        public HttpResponseMessage uploadAvatar()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" }); ;
            }
            var curId = isAuthorized.Data;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatar(curId.ToString(), httpRequest.Files[0]));
            }
            return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatar(curId.ToString(), null));
        }
        
        [HttpPost]
        [Route("upload-finish")]
        public HttpResponseMessage finishUpload()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            //var curId = isAuthorized.Data;
            HttpRequest request = HttpContext.Current.Request;
            var userId = request.Form["userId"];
            var fileName = request.Form["fileName"];
            return Request.CreateResponse(HttpStatusCode.OK, fileService.copyAvatarFromTemp(userId, fileName));
        }

        [HttpPost]
        [Route("upload-temp")]
        public HttpResponseMessage uploadAvatarTemp()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            //var curId = isAuthorized.Data;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatarTemp(httpRequest.Files[0]));
            }
            return Request.CreateResponse(HttpStatusCode.OK, fileService.uploadAvatarTemp(null));
        }
    }
}