using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Services
{
    
    public class RequestWorkflowService
    {
        private MyDbContext db = new MyDbContext();

        public Result<List<RequestWorkflowDTO>> GetRequestWorkflowByRequestId (string requestId)
        {
            List<RequestWorkflowDTO> requestWorkflows =
                db.RequestWorkflows.Where(rwf => rwf.RequestId.ToString() == requestId && rwf.IsDeleted == false)
                .Select(rwf => new RequestWorkflowDTO()
                {
                    Id = rwf.Id,
                    User = new AccountDTO()
                    {
                        Id = rwf.User.Id,
                        FirstName = rwf.User.FirstName,
                        LastName = rwf.User.LastName,
                        Username = rwf.User.Username,
                        JobTitle = rwf.User.JobTitle
                    },
                    Level = rwf.Level,
                    Status = rwf.Status
                })
                .ToList();

            return new Result<List<RequestWorkflowDTO>>(true, "Get Success", requestWorkflows);
        }

        public Result<List<RequestWorkflow>> CreateRequestWorkflow (Guid requestId, string[] listOfUserId)
        {
            List<RequestWorkflow> requestWorkflows = new List<RequestWorkflow>();
            foreach (var userId in listOfUserId)
            {
                RequestWorkflow requestWorkflow = new RequestWorkflow();
                requestWorkflow.UserId = Guid.Parse(userId.ToString());
                requestWorkflow.RequestId = requestId;
                requestWorkflows.Add(requestWorkflow);
            }
            int level = 1;
            if (requestWorkflows.Count == 0)
            {
                return new Result<List<RequestWorkflow>>(false, "Missing Field");
            }

            foreach (RequestWorkflow requestWorkflow in requestWorkflows)
            {
                if (requestWorkflow.UserId == null || requestWorkflow.RequestId == null)
                {
                    return new Result<List<RequestWorkflow>>(false, "Missing UserId or RequestId");
                }
                if (db.Users.SingleOrDefault(u => u.Id == requestWorkflow.UserId && u.IsDeleted == false) == null)
                {
                    return new Result<List<RequestWorkflow>>(false, "User Not Found");
                }
                if (db.Requests.SingleOrDefault(r => r.Id == requestWorkflow.RequestId && r.IsDeleted == false) == null)
                {
                    return new Result<List<RequestWorkflow>>(false, "Request Not Found");
                }
                requestWorkflow.IsDeleted = false;
                requestWorkflow.Level = level;
                requestWorkflow.Status = "Waiting for Approval";
                level += 1;
                db.RequestWorkflows.Add(requestWorkflow);
                db.SaveChanges();
            }
            return new Result<List<RequestWorkflow>>(true, "Create Request Workflow Success", requestWorkflows);
        }

        public Result<List<RequestWorkflow>> EditRequestWorkflow(Guid requestId, string[] listOfUserId)
        {
            var existRequestWorkflows = db.RequestWorkflows.Where(e => e.IsDeleted == false && e.RequestId == requestId).ToList();
            foreach (RequestWorkflow existRequestWorkflow in existRequestWorkflows)
            {
                DeleteRequestWorkflow(existRequestWorkflow.Id);
            }

            var requestWorkflows = CreateRequestWorkflow(requestId, listOfUserId);

            return new Result<List<RequestWorkflow>>(true, "Edit Request Workflow Success", requestWorkflows.Data);
            /*List<RequestWorkflow> requestWorkflows = new List<RequestWorkflow>();
            foreach (var userId in listOfUserId)
            {
                RequestWorkflow requestWorkflow = new RequestWorkflow();
                requestWorkflow.UserId = Guid.Parse(userId.ToString());
                requestWorkflow.RequestId = requestId;
                requestWorkflows.Add(requestWorkflow);
            }
            int level = 1;
            if (requestWorkflows.Count == 0)
            {
                return new Result<List<RequestWorkflow>>(false, "Missing Field");
            }

            foreach (RequestWorkflow requestWorkflow in requestWorkflows)
            {
                if (requestWorkflow.UserId == null || requestWorkflow.RequestId == null)
                {
                    return new Result<List<RequestWorkflow>>(false, "Missing UserId or RequestId");
                }
                if (db.Users.SingleOrDefault(u => u.Id == requestWorkflow.UserId && u.IsDeleted == false) == null)
                {
                    return new Result<List<RequestWorkflow>>(false, "User Not Found");
                }
                if (db.Requests.SingleOrDefault(r => r.Id == requestWorkflow.RequestId && r.IsDeleted == false) == null)
                {
                    return new Result<List<RequestWorkflow>>(false, "Request Not Found");
                }
                requestWorkflow.IsDeleted = false;
                requestWorkflow.Level = level;
                requestWorkflow.Status = "Waiting for Approval";
                level += 1;
                db.RequestWorkflows.Add(requestWorkflow);
                db.SaveChanges();
                *//*var existRequestWorkflow = db.RequestWorkflows.SingleOrDefault(rwf => rwf.IsDeleted == false
                                            && rwf.RequestId == requestWorkflow.RequestId
                                            && rwf.UserId == requestWorkflow.UserId);
                if (existRequestWorkflow != null)
                {
                    if (existRequestWorkflow.Level != level)
                    {
                        existRequestWorkflow.Level = level;
                    }
                }*//*
            }*/

        }

        public Result<string> DeleteRequestWorkflow (Guid Id)
        {
            var requestWorkflow = db.RequestWorkflows.SingleOrDefault(rwf => rwf.IsDeleted == false
            && rwf.Id == Id);

            if (requestWorkflow == null)
            {
                return new Result<string>(false, "Request Workflow Not Found");
            }
            requestWorkflow.IsDeleted = true;
            db.SaveChanges();

            return new Result<string>(true, "Delete Request Workflow has Id = " + requestWorkflow.Id.ToString());
        }

    }
}