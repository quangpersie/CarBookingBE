using CarBookingBE.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Controllers
{
    public class RequestCommentDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public AccountDTO Account { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public Guid? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
        public bool IsDeleted { get; set; }
    }
}