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
    public class RequestCommentsController : ApiController
    {
        private MyDbContext db = new MyDbContext();

        // GET: api/RequestComments
        public IQueryable<RequestComment> GetRequestComments()
        {
            return db.RequestComments;
        }

        // GET: api/RequestComments/5
        [ResponseType(typeof(RequestComment))]
        public IHttpActionResult GetRequestComment(Guid id)
        {
            RequestComment requestComment = db.RequestComments.Find(id);
            if (requestComment == null)
            {
                return NotFound();
            }

            return Ok(requestComment);
        }

        // PUT: api/RequestComments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRequestComment(Guid id, RequestComment requestComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != requestComment.Id)
            {
                return BadRequest();
            }

            db.Entry(requestComment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestCommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RequestComments
        [ResponseType(typeof(RequestComment))]
        public IHttpActionResult PostRequestComment(RequestComment requestComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RequestComments.Add(requestComment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RequestCommentExists(requestComment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = requestComment.Id }, requestComment);
        }

        // DELETE: api/RequestComments/5
        [ResponseType(typeof(RequestComment))]
        public IHttpActionResult DeleteRequestComment(Guid id)
        {
            RequestComment requestComment = db.RequestComments.Find(id);
            if (requestComment == null)
            {
                return NotFound();
            }

            db.RequestComments.Remove(requestComment);
            db.SaveChanges();

            return Ok(requestComment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RequestCommentExists(Guid id)
        {
            return db.RequestComments.Count(e => e.Id == id) > 0;
        }
    }
}