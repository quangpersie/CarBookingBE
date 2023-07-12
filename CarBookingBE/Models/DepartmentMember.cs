using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarBookingTest.Models
{
    public class DepartmentMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Position { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account User { get; set; }
        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}