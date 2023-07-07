using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("AccountRole")]
    public class AccountRole
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}