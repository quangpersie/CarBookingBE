using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class PageDTO<T>
    {
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }

        public List<T> Requests { get; set; }
    }
}