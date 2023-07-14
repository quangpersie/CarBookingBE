using CarBookingBE.DTOs;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using CarBookingBE.Utils;

namespace CarBookingBE.Services
{
    public class RequestService
    {
        private MyDbContext db = new MyDbContext();


        public Result<List<RequestDTO>> GetAllRequests(int page, int limit)
        {
            List<RequestDTO> queries = db.Requests.Include(r => r.SenderUser).Include(receiver => receiver.ReceiveUser)
                .Where(request => request.IsDeleted == false)
                .Select(req => new RequestDTO()
                {
                    Id = req.Id,
                    RequestCode = req.RequestCode,
                    SenderUser = new AccountDTO()
                    {
                        Id = req.SenderUser.Id,
                        FirstName = req.SenderUser.FirstName,
                        LastName = req.SenderUser.LastName
                    },
                    Created = req.Created,
                    Department = new DepartmentDTO()
                    {
                        Name = req.Department.Name
                    },
                    ReceiveUser = new AccountDTO()
                    {
                        Id = req.ReceiveUser.Id,
                        FirstName = req.ReceiveUser.FirstName,
                        LastName = req.ReceiveUser.LastName
                    },
                    UsageFrom = req.UsageFrom,
                    UsageTo = req.UsageTo,
                    Status = req.Status

                })
                .OrderByDescending(request => request.Created)
                .Skip(getSkip(page, limit))
                .Take(limit)
                .ToList();
            return new Result<List<RequestDTO>>(true, "Get Success", queries);
        }

        public Result<RequestDetailDTO> GetRequestById(string id)
        {
            List<RequestWorkflowDTO> requestWorkflow = db.RequestWorkflows.Where(r => r.RequestId.ToString() == id).Include(u => u.User)
                    .Select(rwf => new RequestWorkflowDTO()
                    {
                        Level = rwf.Level,
                        User = new AccountDTO()
                        {
                            Id = rwf.User.Id,
                            FirstName = rwf.User.FirstName,
                            LastName = rwf.User.LastName
                        }
                    }).ToList();
            RequestDetailDTO request = db.Requests.Include(s => s.SenderUser).Include(r => r.ReceiveUser)
                .Select(req => new RequestDetailDTO()
                {
                    Id = req.Id,
                    RequestCode = req.RequestCode,
                    SenderUser = new AccountDTO()
                    {
                        Id = req.SenderUser.Id,
                        FirstName = req.SenderUser.FirstName,
                        LastName = req.SenderUser.LastName
                    },
                    Created = req.Created,
                    Department = new DepartmentDTO()
                    {
                        Name = req.Department.Name
                    },
                    ReceiveUser = new AccountDTO()
                    {
                        Id = req.ReceiveUser.Id,
                        FirstName = req.ReceiveUser.FirstName,
                        LastName = req.ReceiveUser.LastName
                    },
                    RequestWorkflow = requestWorkflow,
                    UsageFrom = req.UsageFrom,
                    UsageTo = req.UsageTo,
                    Status = req.Status,
                    Mobile = req.Mobile,
                    CostCenter = req.CostCenter,
                    TotalPassengers = req.TotalPassengers,
                    PickLocation = req.PickLocation,
                    Destination = req.Destination,
                    Reason = req.Reason,
                    ShareUser = req.ShareUser,
                    Note = req.Note,
                    ApplyNote = req.ApplyNote

                })
                .SingleOrDefault(r => r.Id.ToString() == id);
            if (request == null || request.IsDeleted == true)
            {
                return new Result<RequestDetailDTO>(false, "Failed");
            }

            return new Result<RequestDetailDTO>(true, "Success", request);
        }

        public Result<List<RequestDTO>> GetSentToMe(string userId, int page, int limit)
        {
            var queries = db.Requests.Include(s => s.SenderUser).Include(r => r.ReceiveUser)
                .Join(db.RequestWorkflows, r => r.Id, rwf => rwf.RequestId, (r, rwf) => new { r, rwf })
                .Where(request => request.r.IsDeleted == false)
                .Where(request => request.r.SenderId.ToString() == userId || request.r.ReceiverId.ToString() == userId || request.rwf.UserId.ToString() == userId)
                .Select(req => new RequestDTO()
                {
                    Id = req.r.Id,
                    RequestCode = req.r.RequestCode,
                    SenderUser = new AccountDTO()
                    {
                        Id = req.r.SenderUser.Id,
                        FirstName = req.r.SenderUser.FirstName,
                        LastName = req.r.SenderUser.LastName
                    },
                    Created = req.r.Created,
                    Department = new DepartmentDTO()
                    {
                        Name = req.r.Department.Name
                    },
                    ReceiveUser = new AccountDTO()
                    {
                        Id = req.r.ReceiveUser.Id,
                        FirstName = req.r.ReceiveUser.FirstName,
                        LastName = req.r.ReceiveUser.LastName
                    },
                    UsageFrom = req.r.UsageFrom,
                    UsageTo = req.r.UsageTo,
                    Status = req.r.Status
                })
                .OrderByDescending(request => request.Created)
                .Skip(getSkip(page, limit))
                .Take(limit)
                .ToList();

/*            if (queries.Count() == 0)
            {
                return new Result<List<RequestDTO>>(false, "Get Requests Failed");
            }*/
            return new Result<List<RequestDTO>>(true,"Get Requests Success",queries);
        }

        public Result<List<RequestDTO>> GetSentToOthers(string userId, int page, int limit)
        {
            var queries = db.Requests.Include(s => s.SenderUser).Include(r => r.ReceiveUser)
                        .Where(request => request.IsDeleted == false)
                        .Where(request => request.SenderId.ToString() == userId)
                        .Select(
                            req => new RequestDTO()
                            {
                                Id = req.Id,
                                RequestCode = req.RequestCode,
                                SenderUser = new AccountDTO()
                                {
                                    Id = req.SenderUser.Id,
                                    FirstName = req.SenderUser.FirstName,
                                    LastName = req.SenderUser.LastName
                                },
                                Created = req.Created,
                                Department = new DepartmentDTO()
                                {
                                    Name = req.Department.Name
                                },
                                ReceiveUser = new AccountDTO()
                                {
                                    Id = req.ReceiveUser.Id,
                                    FirstName = req.ReceiveUser.FirstName,
                                    LastName = req.ReceiveUser.LastName
                                },
                                UsageFrom = req.UsageFrom,
                                UsageTo = req.UsageTo,
                                Status = req.Status
                            }
                        )
                        .ToList();
            return new Result<List<RequestDTO>>(true, "Get Requests Success", queries);
        }

        public Result<Request> EditRequest(string id, Request requestEdit)
        {
            var req = db.Requests.SingleOrDefault(r => r.Id == Guid.Parse(id));
            if (req == null || req.IsDeleted == true)
            {
                return new Result<Request>(false, "Request Not Found");
            }

            if (Guid.Parse(id) != req.Id)
            {
                return new Result<Request>(false, "Request id not match");
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

            return new Result<Request>(true, "Edit Request Succcess!", req);
        }

        public Result<Request> CreateRequest(Request request)
        {
            request.Created = DateTime.Now;
            request.IsDeleted = false;
            request.RequestCode = GenerateRequestCode();
            request.Status = "Waiting for Approval";
            if (request.Mobile == null || request.CostCenter == null || request.TotalPassengers == null || request.UsageFrom == null 
                || request.UsageTo == null || request.PickTime == null || request.PickLocation == null
                || request.Destination == null || request.Reason == null || request.ApplyNote == null)
            {
                return new Result<Request>(false, "Missing Fields", request);
            }

            if (db.Users.SingleOrDefault(u => u.Id == request.SenderId && u.IsDeleted == false) == null)
            {
                return new Result<Request>(false, "Sender User Not Exist");
            }

            if (db.Departments.SingleOrDefault(d => d.Id == request.DepartmentId && d.IsDeleted == false) == null)
            {
                return new Result<Request>(false, "Department Not Exist");
            } 

            if (db.Users.SingleOrDefault(u => u.Id == request.ReceiverId && u.IsDeleted == false) == null)
            {
                return new Result<Request>(false, "Receiver User Not Exist");
            }

            db.Requests.Add(request);
            db.SaveChanges();
            return new Result<Request>(true, "Create Request Success!", request);
        }

        public Result<Request> DeleteRequest(string id)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id == Guid.Parse(id));
            if (request == null || request.IsDeleted == true)
            {
                return new Result<Request>(false, "Request Not Found");
            }
            request.IsDeleted = true;
            db.SaveChanges();

            return new Result<Request>(true, "Delete Success Request has RequestCode = " + request.RequestCode);
        }

        public Result<List<Request>> FilterRequest(string requestCode, string createdFrom, string createdTo, string senderId, string status, int page, int limit)
        {
            var requestList = db.Requests.Where(req => req.IsDeleted == false);
            if (requestCode != null)
            {
                requestList = requestList.Where(req => req.RequestCode.Contains(requestCode));
            }
            if (createdFrom != null && createdTo != null)
            {
                requestList = requestList.Where(req => req.Created > DateTime.Parse(createdFrom) && req.Created < DateTime.Parse(createdTo));
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
            return new Result<List<Request>>(true, "Filter Success", result);
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

            foreach (Request request in db.Requests)
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

    }




}