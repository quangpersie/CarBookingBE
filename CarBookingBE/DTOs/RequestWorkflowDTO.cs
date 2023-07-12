using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class RequestWorkflowDTO
    {
        public AccountDTO User { get; set; }
        public int Level { get; set; }
    }
}