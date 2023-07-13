using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IHttpActionResult CreateComment (RequestComment requestComment, string requestId)
        {
            return Ok();
        }
    }
}