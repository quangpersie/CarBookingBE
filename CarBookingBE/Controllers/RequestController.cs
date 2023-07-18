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
        public IHttpActionResult EditRequest(string id)
        {
            var httpRequest = HttpContext.Current.Request;
            Request request = new Request();
            if (httpRequest.Form["SenderId"] != null) request.SenderId = Guid.Parse(httpRequest.Form["SenderId"]);
            if (httpRequest.Form["DepartmentId"] != null) request.DepartmentId = Guid.Parse(httpRequest.Form["DepartmentId"]);
            if (httpRequest.Form["ReceiverId"] != null) request.ReceiverId = Guid.Parse(httpRequest.Form["ReceiverId"]);
            if (httpRequest.Form["Mobile"] != null) request.Mobile = httpRequest.Form["Mobile"];
            if (httpRequest.Form["CostCenter"] != null) request.CostCenter = httpRequest.Form["CostCenter"];
            if (httpRequest.Form["TotalPassengers"] != null) request.TotalPassengers = int.Parse(httpRequest.Form["TotalPassengers"]);
            if (httpRequest.Form["UsageFrom"] != null) request.UsageFrom = DateTime.Parse(httpRequest.Form["UsageFrom"]);
            if (httpRequest.Form["UsageTo"] != null) request.UsageTo = DateTime.Parse(httpRequest.Form["UsageTo"]);
            if (httpRequest.Form["PickTime"] != null) request.PickTime = DateTime.Parse(httpRequest.Form["PickTime"]);
            if (httpRequest.Form["PickLocation"] != null) request.PickLocation = httpRequest.Form["PickLocation"];
            if (httpRequest.Form["Destination"] != null) request.Destination = httpRequest.Form["Destination"];
            if (httpRequest.Form["Reason"] != null) request.Reason = httpRequest.Form["Reason"];
            if (httpRequest.Form["ApplyNote"] != null) request.ApplyNote = bool.Parse(httpRequest.Form["ApplyNote"]);
            if (httpRequest.Form["Note"] != null) request.Note = httpRequest.Form["Note"];
            var requestEdit = requestService.EditRequest(id, request);

            var requestId = requestEdit.Data.Id;

            // Edit RequestWorkflow
            if (httpRequest.Form["ListOfUserId"] != null)
            {
                var listOfUserId = httpRequest.Params.GetValues("ListOfUserId");

                var createRequestWorkflow = requestWorkflowService.EditRequestWorkflow(requestId, listOfUserId);
                if (!createRequestWorkflow.Success)
                {
                    return BadRequest(createRequestWorkflow.Message);
                }
            }

            // Edit RequestAttachment
            if (httpRequest.Files.Count > 0)
            {
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    
                    var createAttachment = requestAttachmentService.EditAttachment(httpRequest.Files[i], requestId);
                    if (!createAttachment.Success)
                    {
                        return BadRequest(createAttachment.Message);
                    }
                }
                /*var requestAttachments = requestAttachmentService.GetAttachmentByRequestId(requestId.ToString());
                foreach(RequestAttachmentDTO requestAttachment in requestAttachments.Data)
                {
                    if (!newRequestAttachments.Exists(nr => nr.Path == requestAttachment.Path))
                    {
                        requestAttachmentService.deleteAttachment(requestAttachment.Id);
                    }
                }*/
            }

            return Ok(requestEdit);
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
            Guid requestId = Guid.Parse(id);
            requestWorkflowService.DeleteAllRequestWorkflows(requestId);
            requestAttachmentService.DeleteAllAttachments(requestId);
            requestService.DeleteRequest(requestId);
            return Ok();
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
            if (httpRequest.Form["Status"] != null) request.Status = httpRequest.Form["Status"];
            
            var newRequest = requestService.CreateRequest(request);

            if (newRequest.Success)
            {
                // Create RequestWorkflow
                if (httpRequest.Form["ListOfUserId"] != null)
                {
                    var requestId = newRequest.Data.Id;
                    var listOfUserId = httpRequest.Params.GetValues("ListOfUserId");
                    /*List<RequestWorkflow> requestWorkflows = new List<RequestWorkflow>();
                    foreach (var userId in httpRequest.Params.GetValues("ListOfUserId"))
                    {
                        RequestWorkflow requestWorkflow = new RequestWorkflow();
                        requestWorkflow.UserId = Guid.Parse(userId.ToString());
                        requestWorkflow.RequestId = newRequest.Data.Id;
                        requestWorkflows.Add(requestWorkflow);
                    }
*/
                    var createRequestWorkflow = requestWorkflowService.CreateRequestWorkflow(requestId, listOfUserId);
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

        [Route("action/Id={Id}")]
        [HttpPut]
        public IHttpActionResult ActionRequest(string Id)
        {
            var httpRequest = HttpContext.Current.Request;
            string userId = httpRequest.Form["userId"];
            string action = httpRequest.Form["action"];
            string Note = httpRequest.Form["Note"];
            var requestWorkflow = requestWorkflowService.ActionRequest(Guid.Parse(Id), Guid.Parse(userId), action);
            if (!requestWorkflow.Success)
            {
                return BadRequest(requestWorkflow.Message);
            }

            var checkWorkflow = requestWorkflowService.CheckWorkflow(requestWorkflow.Data);
            if (checkWorkflow.Success)
            {
                var actionRequest = requestService.ActionRequest(Id, Note, userId, action);
                if (!actionRequest.Success)
                {
                    return BadRequest(actionRequest.Message);
                }
                return Ok(actionRequest.Message);

            }


            return Ok(requestWorkflow.Message);
        }

    }
}