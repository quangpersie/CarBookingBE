using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class LoginReturnDTO
    {
        public Guid userId { get; set; }
        public string jwtToken { get; set; }
        public LoginReturnDTO(Guid userId, string jwtToken)
        {
            this.userId = userId;
            this.jwtToken = jwtToken;
        }
    }
}