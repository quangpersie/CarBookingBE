using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestAttachment")]
    public class RequestAttachment
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
        public bool isDeleted { get; set; }
    }
}