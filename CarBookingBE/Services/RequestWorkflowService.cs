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
                db.RequestWorkflows.Where(rwf => rwf.RequestId.ToString() == requestId)
                .Select(rwf => new RequestWorkflowDTO()
                {
                    Id = rwf.Id,
                    User = new AccountDTO()
                    {
                        Id = rwf.User.Id,
                        FirstName = rwf.User.FirstName,
                        LastName = rwf.User.LastName
                    },
                    Level = rwf.Level,
                    Status = rwf.Status
                })
                .ToList();

            return new Result<List<RequestWorkflowDTO>>(true, "Get Success", requestWorkflows);
        }

        public Result<List<RequestWorkflow>> CreateRequestWorkflow (List<RequestWorkflow> requestWorkflows)
        {
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
    }
}