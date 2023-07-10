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
using CarBookingTest.Models;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request")]
    public class RequestController : ApiController
    {
        private MyDbContext db = new MyDbContext();
        List<Request> requests;
        // GET: Request

        public RequestController()
        {
            requests = db.Requests.ToList();
        }


        // GET: api/Request
        [Route("get-all")]
        [HttpGet]
        public IHttpActionResult GetRequests(int page, int limit)
        {
            return Ok(requests.Where(request => request.IsDeleted == false)
                .OrderByDescending(request => request.Created)
                .Skip(getSkip(page, limit))
                .Take(limit)
                .ToList());
        }

        // GET: api/Request/5
        [Route("Id={id}")]
        [HttpGet]
        public IHttpActionResult GetRequest(string id)
        {

            Request request = db.Requests.SingleOrDefault(r => r.Id.ToString() == id);
            if (request == null || request.IsDeleted == true)
            {
                return NotFound();
            }

            return Ok(request);
        }

        // GET: Sent to me
        [Route("sent-to-me/{Id}")]
        [HttpGet]

        public IHttpActionResult GetSentToMe(string Id)
        {
            List<Request> requestList = requests.Where(request => request.SenderId.ToString() == Id
                || request.ReceiverId.ToString() == Id
            )
                .ToList();
            return Ok();
        }

        // PUT: api/Request/5
        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult EditRequest(string id, Request requestEdit)
        {
            try
            {
                var req = requests.SingleOrDefault(r => r.Id == Guid.Parse(id));
                if (req == null || req.IsDeleted == true)
                {
                    return NotFound();
                }

                if (Guid.Parse(id) != req.Id)
                {
                    return BadRequest();
                }
                if (requestEdit.DepartmentId != null) req.DepartmentId = requestEdit.DepartmentId;
                if (requestEdit.ReceiverId != null) req.ReceiverId = requestEdit.ReceiverId;
                if (requestEdit.Mobile != null) req.Mobile = requestEdit.Mobile;
                if (requestEdit.CostCenter != null) req.CostCenter = requestEdit.CostCenter;
                if (requestEdit.TotalPassengers != 0) req.TotalPassengers = requestEdit.TotalPassengers;
                if (requestEdit.UsageFrom != null) req.UsageFrom = requestEdit.UsageFrom;
                if (requestEdit.UsageTo != null) req.UsageTo = requestEdit.UsageTo;
                if (requestEdit.PickTime != null) req.PickTime = requestEdit.PickTime;
                if (requestEdit.PickLocation != null) req.PickLocation = requestEdit.PickLocation;
                if (requestEdit.Destination != null) req.Destination = requestEdit.Destination;
                if (requestEdit.Reason != null) req.Reason = requestEdit.Reason;
                if (requestEdit.ApplyNote != null) req.ApplyNote = requestEdit.ApplyNote;
                req.Created = DateTime.Now;

                db.SaveChanges();
                return Ok(req);
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: api/Request
        [Route("create")]
        [HttpPost]
        public IHttpActionResult CreateRequest(Request request)
        {
            try
            {
                request.Created = DateTime.Now;
                request.IsDeleted = false;
                request.RequestCode = GenerateRequestCode();
                db.Requests.Add(request);
                db.SaveChanges();

                return Ok(request);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/Request/5
        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteRequest(string id)
        {
            try
            {
                Request request = db.Requests.SingleOrDefault(r => r.Id == Guid.Parse(id));
                if (request == null || request.IsDeleted == true)
                {
                    return NotFound();
                }
                request.IsDeleted = true;
                db.SaveChanges();

                return Ok("Delete Success Request has Id = " + id);
            }
            catch
            {
                return BadRequest();
            }

        }

        // -----FILTER-------------------
        [Route("filter")]
        [HttpGet]
        public IHttpActionResult FilterRequest(string requestCode, string createdTime, string senderId, string status, int page, int limit)
        {
            var requestList = requests.Where(req => req.IsDeleted == false);
            if (requestCode != null)
            {
                requestList = requestList.Where(req => req.RequestCode.Contains(requestCode));
            }
            if (createdTime != null)
            {
                requestList = requestList.Where(req => req.Created > DateTime.Parse(createdTime));
            }
            if (senderId != null)
            {
                requestList = requestList.Where(req => req.SenderId.ToString() == senderId);
            }
            if (senderId != null)
            {
                requestList = requestList.Where(req => req.Status == status);
            }
            List<Request> result = requestList.OrderBy(req => req.Created).Skip(getSkip(page, limit)).Take(limit).ToList();
            return Ok(result);
        }


        // ---------------------------Function----------------------------------------
        protected string GenerateRequestCode()
        {
            string year = DateTime.Now.Year.ToString();
            string companyCode = "OPS";
            string module = "CAR";
            string month = DateTime.Now.Month.ToString("d2");
            string day = DateTime.Now.Day.ToString("d2");
            string requestCodeBase = year + companyCode + "-" + module + "-" + month + day + "-";
            List<string> requestCodeList = new List<string>();
            List<int> requestCodeNumbers = new List<int>();
            int maxNumber = 0;

            foreach (Request request in requests)
            {
                if (request.RequestCode.Contains(requestCodeBase) && request.IsDeleted == false)
                {
                    requestCodeList.Add(request.RequestCode);
                }
            }

            if (requestCodeList.Count() != 0)
            {
                foreach (string requestCode in requestCodeList)
                {
                    string requestCodeString = requestCode.Substring(requestCode.Length - 3);
                    requestCodeNumbers.Add(int.Parse(requestCodeString));
                }
                maxNumber = requestCodeNumbers.Max();
            }

            return requestCodeBase + (maxNumber + 1).ToString("000");
        }

        //--------------------Pagination ----------------------------------
        protected int getSkip(int pageIndex, int limit)
        {
            return (pageIndex - 1) * limit;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RequestExists(string id)
        {
            return db.Requests.Count(e => e.Id == Guid.Parse(id)) > 0;
        }

    }
}