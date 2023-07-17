using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using Newtonsoft.Json;
using CarBookingBE.Services;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request")]
    public class RequestController : ApiController
    {
        RequestService requestService = new RequestService();
        RequestAttachmentService requestAttachmentService = new RequestAttachmentService();
        RequestWorkflowService requestWorkflowService = new RequestWorkflowService();


        // GET: api/Request
        [Route("get-all")]
        [HttpGet]
        public IHttpActionResult GetRequests(string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetAllRequests(page, limit);
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }

        // GET: api/Request/5
        [Route("Id={id}")]
        [HttpGet]
        public IHttpActionResult GetRequest(string id)
        {
            var request = requestService.GetRequestById(id);
            if (!request.Success)
            {
                return BadRequest(request.Message);
            }
            return Ok(request);
        }

        // GET: Sent to me Not Complete
        [Route("sent-to-me/userId={userId}")]
        [HttpGet]

        public IHttpActionResult GetSentToMe(string userId, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetSentToMe(userId, page, limit);
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }

        // GET: Sent to others Not Complete
        [Route("sent-to-others/userId={userId}")]
        [HttpGet]

        public IHttpActionResult GetSentToOthers(string userId, string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetSentToOthers(userId, page, limit);
            if (!requestList.Success)
            {
                return BadRequest(requestList.Message);
            }
            return Ok(requestService.FilterRequest(requestList.Data, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }


        // PUT: api/Request/5
        [Route("Id={id}")]
        [HttpPut]
        public IHttpActionResult EditRequest(string id, Request requestEdit)
        {
            var request = requestService.EditRequest(id, requestEdit);
            return Ok();
        }

        /*// POST: api/Request
        [Route("create")]
        [HttpPost]
        public IHttpActionResult CreateRequest(Request request)
        {
            return Ok(requestService.CreateRequest(request));
        }*/

        // DELETE: api/Request/5
        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteRequest(string id)
        {
            return Ok(requestService.DeleteRequest(id));
        }

        [Route("create")]
        [HttpPost]
        public IHttpActionResult CreateRequest()
        {
            var httpRequest = HttpContext.Current.Request;
            var checkKeys = requestService.CheckKeysRequired(httpRequest.Form.AllKeys);
            if (!checkKeys.Success)
            {
                return BadRequest(checkKeys.Message);
            }
            Request request = new Request();
            request.SenderId = Guid.Parse(httpRequest.Form["SenderId"]);
            request.DepartmentId = Guid.Parse(httpRequest.Form["DepartmentId"]);
            request.ReceiverId = Guid.Parse(httpRequest.Form["ReceiverId"]);
            request.Mobile = httpRequest.Form["Mobile"];
            request.CostCenter = httpRequest.Form["CostCenter"];
            request.TotalPassengers = int.Parse(httpRequest.Form["TotalPassengers"]);
            request.UsageFrom = DateTime.Parse(httpRequest.Form["UsageFrom"]);
            request.UsageTo = DateTime.Parse(httpRequest.Form["UsageTo"]);
            request.PickTime = DateTime.Parse(httpRequest.Form["PickTime"]);
            request.PickLocation = httpRequest.Form["PickLocation"];
            request.Destination = httpRequest.Form["Destination"];
            request.Reason = httpRequest.Form["Reason"];
            request.ApplyNote = bool.Parse(httpRequest.Form["ApplyNote"]);
            if (httpRequest.Form["Note"] != null) request.Note = httpRequest.Form["Note"];
            
            var newRequest = requestService.CreateRequest(request);

            if (newRequest.Success)
            {
                // Create RequestWorkflow
                if (httpRequest.Form["ListOfUserId"] != null)
                {
                    List<RequestWorkflow> requestWorkflows = new List<RequestWorkflow>();
                    foreach (var userId in httpRequest.Params.GetValues("ListOfUserId"))
                    {
                        RequestWorkflow requestWorkflow = new RequestWorkflow();
                        requestWorkflow.UserId = Guid.Parse(userId.ToString());
                        requestWorkflow.RequestId = newRequest.Data.Id;
                        requestWorkflows.Add(requestWorkflow);
                    }

                    var createRequestWorkflow = requestWorkflowService.CreateRequestWorkflow(requestWorkflows);
                    if (!createRequestWorkflow.Success)
                    {
                        return BadRequest(createRequestWorkflow.Message);
                    }
                }

                // Create RequestAttachment
                if (httpRequest.Files.Count > 0)
                {
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        var createAttachment = requestAttachmentService.CreateAttachment(httpRequest.Files[i], newRequest.Data.Id);
                        if (!createAttachment.Success)
                        {
                            return BadRequest(createAttachment.Message);
                        }

                    }
                }
            }
            return Ok(newRequest);
        }

        /*// -----FILTER-------------------
        [Route("filter")]
        [HttpGet]
        public IHttpActionResult FilterRequestAll(string requestCode, string createdFrom,string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = requestService.GetAllRequests(page, limit).Data;
            return Ok(requestService.FilterRequest(requestList, requestCode, createdFrom, createdTo, senderId, status, page, limit));
        }*/

    }
}