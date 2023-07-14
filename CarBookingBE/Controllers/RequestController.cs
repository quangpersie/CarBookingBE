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

        // POST: api/Request
        [Route("create")]
        [HttpPost]
        public IHttpActionResult CreateRequest(Request request)
        {
            return Ok(requestService.CreateRequest(request));
        }

        // DELETE: api/Request/5
        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteRequest(string id)
        {
            return Ok(requestService.DeleteRequest(id));
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