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
        public IHttpActionResult GetRequests()
        {
            return Ok(requests.ToList());
        }

        // GET: api/Request/5
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult GetRequest(int id)
        {
            Request request = db.Requests.Find(id);
            if (request == null || request.IsDeleted == true)
            {
                return NotFound();
            }

            return Ok(request);
        }

        // PUT: api/Request/5
        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult EditRequest(int id, Request requestEdit)
        {
            var req = requests.SingleOrDefault(r => r.Id == id);
            if (req == null || req.IsDeleted == true)
            {
                return NotFound();
            }

            if (id != req.Id)
            {
                return BadRequest();
            }
            /*if (requestEdit.DepartmentId != null) req.DepartmentId = requestEdit.DepartmentId;
            if (requestEdit.ReceiverId != null) req.ReceiverId = requestEdit.ReceiverId;
            if (requestEdit.Mobile != null) req.Mobile = requestEdit.Mobile;
            if (requestEdit.CostCenter != null) req.CostCenter = requestEdit.CostCenter;
            if (requestEdit.TotalPassengers == 0) req.TotalPassengers = requestEdit.TotalPassengers;
            if (requestEdit.UsageFrom != null) req.UsageFrom = requestEdit.UsageFrom;
            if (requestEdit.UsageTo != null) req.UsageTo = requestEdit.UsageTo;
            if (requestEdit.PickTime != null) req.PickTime = requestEdit.PickTime;
            if (requestEdit.PickLocation != null) req.PickLocation = requestEdit.PickLocation;
            if (requestEdit.Destination != null) req.Destination = requestEdit.Destination;
            if (requestEdit.Reason != null) req.Reason = requestEdit.Reason;
            if (requestEdit.ApplyNote != null) req.ApplyNote = requestEdit.ApplyNote;*/
            if (requestEdit.Mobile != null) req.Mobile = requestEdit.Mobile;
            if (requestEdit.TotalPassengers != 0) req.TotalPassengers = requestEdit.TotalPassengers;
            req.Created = DateTime.Now;

            db.SaveChanges();
            return Ok(req);
            /*try
            {
                var req = requests.SingleOrDefault(r => r.Id == id);
                if (req == null || req.IsDeleted == true)
                {
                    return NotFound();
                }

                if (id != req.Id)
                {
                    return BadRequest();
                }
                if (requestEdit.DepartmentId != null) req.DepartmentId = requestEdit.DepartmentId;
                if (requestEdit.ReceiverId != null) req.ReceiverId = requestEdit.ReceiverId;
                if (requestEdit.Mobile != null) req.Mobile = requestEdit.Mobile;
                if (requestEdit.CostCenter != null) req.CostCenter = requestEdit.CostCenter;
                if (requestEdit.TotalPassengers == 0) req.TotalPassengers = requestEdit.TotalPassengers;
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
            }*/
        }

        // POST: api/Request
        [ResponseType(typeof(Request))]
        public IHttpActionResult PostRequest(Request request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Requests.Add(request);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = request.Id }, request);
        }

        // DELETE: api/Request/5
        [ResponseType(typeof(Request))]
        public IHttpActionResult DeleteRequest(int id)
        {
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return NotFound();
            }

            db.Requests.Remove(request);
            db.SaveChanges();

            return Ok(request);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RequestExists(int id)
        {
            return db.Requests.Count(e => e.Id == id) > 0;
        }
    }
}