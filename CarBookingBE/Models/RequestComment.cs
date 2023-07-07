using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestComment")]
    public class RequestComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
        public bool IsDeleted { get; set; }
    }
}