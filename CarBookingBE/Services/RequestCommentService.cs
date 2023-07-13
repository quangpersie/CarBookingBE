using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Services
{
    public class RequestCommentService
    {
        MyDbContext db = new MyDbContext();

        public Result<List<RequestCommentDTO>> GetAllCommentByRequestId(string requestId)
        {
            Request request = db.Requests.SingleOrDefault(r => r.Id.ToString() == requestId && r.IsDeleted == false);
            if (request == null)
            {
                return new Result<List<RequestCommentDTO>>(false, "Request Not Found");
            }

            List<RequestCommentDTO> requestComments =
                db.RequestComments.Where(rc => rc.RequestId.ToString() == requestId)
                .Select(rc => new RequestCommentDTO()
                {
                    Id = rc.Id,
                    Account = new AccountDTO()
                    {
                        Id = rc.Account.Id,
                        FirstName = rc.Account.FirstName,
                        LastName = rc.Account.LastName
                    },
                    Content = rc.Content,
                    Created = rc.Created
                })
                .ToList();

            return new Result<List<RequestCommentDTO>>(true, "Get Success", requestComments);
        }

        public Result<RequestComment> 
    }
}