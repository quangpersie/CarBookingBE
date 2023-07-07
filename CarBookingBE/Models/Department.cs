using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Code { get; set; }
        public Guid? UnderDepartment { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<DepartmentMember> DepartmentMembers { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}