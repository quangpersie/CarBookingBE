using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    public class DepartmentMember
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public bool isDeleted { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}