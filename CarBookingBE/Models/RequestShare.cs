using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestShare")]
    public class RequestShare
    {
        [Key]
        public string Id { get; set; }
        public bool isDeleted { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
    }
}