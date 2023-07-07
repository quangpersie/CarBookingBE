﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("Request")]
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string RequestCode { get; set; }
        public int? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public Account SenderUser { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public int? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public Account ReceiveUser { get; set; }
        public DateTime? Created { get; set; }
        public string Mobile { get; set; }
        public string CostCenter { get; set; }
        public int TotalPassengers { get; set; }
        public DateTime UsageFrom { get; set; }
        public DateTime UsageTo { get; set; }
        public DateTime PickTime { get; set; }
        public string PickLocation { get; set; }
        public string Destination { get; set; }
        public string Reason { get; set; }
        public int? Share { get; set; }

        public string Note { get; set; }
        public bool? ApplyNote { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<RequestAttachment> RequestAttachments { get; set;}
        public virtual ICollection<RequestComment> RequestComments { get; set;}
        public virtual ICollection<RequestWorkflow> RequestWorkflows { get; set; }
    }
}