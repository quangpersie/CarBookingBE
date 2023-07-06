using System;
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
        public string Sender { get; set; }
        public string Department { get; set; }
        public string Receiver { get; set; }
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
        public bool isDeleted { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public virtual ICollection<RequestAttachment> RequestAttachments { get; set;}
        public virtual ICollection<RequestComment> RequestComments { get; set;}
        public virtual ICollection<RequestWorkflow> RequestWorkflows { get; set; }
    }
}