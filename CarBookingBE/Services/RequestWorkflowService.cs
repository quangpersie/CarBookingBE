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
                var userRoles = db.UserRoles.Where(ur => ur.IsDeleted == false && ur.UserId.ToString() == userId).ToList();
                var roles = new List<string>();
                foreach (AccountRole accountRole in userRoles)
                {
                    var role = db.Roles.SingleOrDefault(r => r.Id == accountRole.RoleId);
                    roles.Add(role.Title);
                }
                foreach (string checkRole in roles)
                {
                    if (checkRole == "ADMIN" || checkRole == "ADMINISTRATIVE" || checkRole == "APPROVER")
                    {
                        RequestWorkflow requestWorkflow = new RequestWorkflow();
                        requestWorkflow.UserId = Guid.Parse(userId.ToString());
                        requestWorkflow.RequestId = requestId;
                        requestWorkflows.Add(requestWorkflow);
                    } else
                    {
                        return new Result<List<RequestWorkflow>>(false, "User Permission Failed");
                    }
                }
                
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
                requestWorkflow.Status = "Waiting for approval";
                level += 1;
                db.RequestWorkflows.Add(requestWorkflow);
                db.SaveChanges();
            }
            return new Result<List<RequestWorkflow>>(true, "Create Request Workflow Success", requestWorkflows);
        }

        public Result<List<RequestWorkflow>> EditRequestWorkflow(Guid requestId, string[] listOfUserId)
        {
            DeleteAllRequestWorkflows(requestId);

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

        public Result<string> DeleteAllRequestWorkflows(Guid requestId)
        {
            var existRequestWorkflows = db.RequestWorkflows.Where(e => e.IsDeleted == false && e.RequestId == requestId).ToList();
            foreach (RequestWorkflow existRequestWorkflow in existRequestWorkflows)
            {
                DeleteRequestWorkflow(existRequestWorkflow.Id);
            }
            return new Result<string>(true, "Delete All Request Workflow Success!");
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

        public Result<RequestWorkflow> ActionRequest(Guid requestId, Guid userId, string action)
        {
            var requestWorkflow = db.RequestWorkflows.SingleOrDefault(rw => rw.IsDeleted == false && rw.RequestId == requestId && rw.UserId == userId);
            if (requestWorkflow == null)
            {
                return new Result<RequestWorkflow>(false, "Request Workflow Not Found");
            }

            if (requestWorkflow.Level > 1)
            {
                var requestWorkflowPreLevel = db.RequestWorkflows.SingleOrDefault(rw => rw.IsDeleted == false && rw.RequestId == requestId && rw.Level == requestWorkflow.Level - 1);
                if (requestWorkflowPreLevel != null)
                {
                    if (requestWorkflowPreLevel.Status == "Waiting for approval")
                    {
                        return new Result<RequestWorkflow>(false, "Please waiting for approval of Previous Approver!");
                    }
                    else if (requestWorkflowPreLevel.Status == "Approved")
                    {
                        requestWorkflow.Status = action;
                        db.SaveChanges();
                        return new Result<RequestWorkflow>(true, "Approved Success!", requestWorkflow);
                    }
                    else
                    {
                        return new Result<RequestWorkflow>(false, "Request is" + requestWorkflowPreLevel.Status);
                    }
                }
            }
            requestWorkflow.Status = action;
            db.SaveChanges();

            return new Result<RequestWorkflow>(true, "Success", requestWorkflow);
        }

        public Result<string> CheckWorkflow(RequestWorkflow requestWorkflowCheck)
        {
            List<RequestWorkflow> requestWorkflows = db.RequestWorkflows.Where(rw => rw.IsDeleted == false
            && rw.RequestId == requestWorkflowCheck.RequestId
            && rw.UserId != requestWorkflowCheck.UserId
            ).ToList();

            if (requestWorkflowCheck.Status != "Approved")
            {
                if (requestWorkflows.Count > 0)
                {
                    foreach (RequestWorkflow requestWorkflow1 in requestWorkflows)
                    {
                        requestWorkflow1.Status = requestWorkflowCheck.Status;
                        db.SaveChanges();
                    }
                }
                
                return new Result<string>(true, requestWorkflowCheck.Status + "Request Success");
            }
            

            if (requestWorkflows.Count > 0)
            {
                foreach (RequestWorkflow requestWorkflow1 in requestWorkflows)
                {
                    if (requestWorkflow1.Level > requestWorkflowCheck.Level)
                    {
                        return new Result<string>(false, "Not yet fully approved");
                    }
                }
            }
            
            return new Result<string>(true, "Fully approved success");
        }

    }
}