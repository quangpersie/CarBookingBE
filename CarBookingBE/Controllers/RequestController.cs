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
            if (request == null || request.isDeleted == true)
            {
                return NotFound();
            }

            return Ok(request);
        }

        // PUT: api/Request/5
        /*[Route("{id}")]
        [HttpPut]
        public IHttpActionResult EditRequest(int id, Request requestEdit)
        {
            try
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
                if (requestEdit.EmployeePassword != null) emp.EmployeePassword = requestEdit.EmployeePassword;
                if (requestEdit.FirstName != null) emp.FirstName = requestEdit.FirstName;
                if (requestEdit.LastName != null) emp.LastName = requestEdit.LastName;
                if (requestEdit.Gender != null) emp.Gender = requestEdit.Gender;
                if (requestEdit.Age != null) emp.Age = requestEdit.Age;
                if (requestEdit.Phone != null) emp.Phone = requestEdit.Phone;
                if (requestEdit.EmployeeAddress != null) emp.EmployeeAddress = requestEdit.EmployeeAddress;
                if (requestEdit.EmployeeNumber != null) emp.EmployeeNumber = requestEdit.EmployeeNumber;
                emp.ModifyAt = DateTime.Now;

                db.SaveChanges();
                return Ok(emp);
            }
            catch
            {
                return BadRequest();
            }
        }*/

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