using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace CarBookingBE.DTOs
{
    public class RoleNameDTO
    {
        public string Admin = "ADMIN";
        public string Administrative = "ADMINISTRATIVE";
        public string Approver = "APPROVER";
        public string Employee = "EMPLOYEE";
        public string Security = "SECURITY";
        public List<string> Roles = new List<string>();
        public RoleNameDTO(string admin, string administrative, string approver, string employee, string security)
        {
            if(Admin == admin) Roles.Add(Admin);
            if (Administrative == administrative) Roles.Add(Security);
            if (Approver == approver) Roles.Add(Administrative);
            if (Employee == employee) Roles.Add(Approver);
            if (Security == security) Roles.Add(Employee);
        }
    }
}