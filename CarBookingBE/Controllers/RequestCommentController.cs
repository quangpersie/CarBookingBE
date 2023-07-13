using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CarBookingBE.Services;
using CarBookingTest.Models;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request")]
    public class RequestCommentController : ApiController
    {
        RequestCommentService requestCommentService = new RequestCommentService();

        [Route("comment/requestId={requestId}")]
        [HttpGet]
        public IHttpActionResult GetAllCommentByRequestId (string requestId)
        {
            var requestComments = requestCommentService.GetAllCommentByRequestId(requestId);
            if (!requestComments.Success)
            {
                return BadRequest(requestComments.Message);
            }
            return Ok(requestComments);
        }

        [Route("comment/requestId={requestId}/create")]
        [HttpPost]
        public IHttpActionResult CreateComment (string requestId)
        {
            var httpRequest = HttpContext.Current.Request;
            RequestComment requestComment = new RequestComment();
            requestComment.UserId = Guid.Parse(httpRequest.Form["UserId"]);
            requestComment.Content = httpRequest.Form["Content"];
            requestComment.Created = DateTime.Now;
            requestComment.RequestId = Guid.Parse(httpRequest.Form["requestId"]);
            requestComment.IsDeleted = false;

            if (httpRequest.Files.Count > 0)
            {
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var comments = requestCommentService.CreateComment(httpRequest.Files[i], requestComment, requestId);
                    if (!comments.Success)
                    {
                        return BadRequest(comments.Message);
                    } 
                }
            }
            else
            {
                var comments = requestCommentService.CreateComment(null, requestComment, requestId);
                if (!comments.Success)
                {
                    return BadRequest(comments.Message);
                }
            }

            return Ok("Create Comment Success");
        }
    }
}