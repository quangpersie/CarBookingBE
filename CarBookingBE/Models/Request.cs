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
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestCode { get; set; }
        public Guid? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public Account SenderUser { get; set; }
        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
        public Guid? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public Account ReceiveUser { get; set; }
        public DateTime? Created { get; set; }
        public string Mobile { get; set; }
        public string CostCenter { get; set; }
        public int? TotalPassengers { get; set; }
        public DateTime? UsageFrom { get; set; }
        public DateTime? UsageTo { get; set; }
        public DateTime? PickTime { get; set; }
        public string PickLocation { get; set; }
        public string Destination { get; set; }
        public string Reason { get; set; }
        public Guid? Share { get; set; }
        [ForeignKey("Share")]
        public Account ShareUser { get; set; }
        public string Note { get; set; }
        public bool? ApplyNote { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<RequestAttachment> RequestAttachments { get; set;}
        public virtual ICollection<RequestComment> RequestComments { get; set;}
        public virtual ICollection<RequestWorkflow> RequestWorkflows { get; set; }
    }
}