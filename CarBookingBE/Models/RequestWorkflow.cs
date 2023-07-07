using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestWorkflow")]
    public class RequestWorkflow
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
    }
}