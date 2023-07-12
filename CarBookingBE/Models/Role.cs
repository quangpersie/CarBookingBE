using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarBookingTest.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public virtual ICollection<AccountRole> UserRoles { get; set; }
    }
}