using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CarBookingBE.Models;
using CarBookingBE.Services;
using CarBookingTest.Models;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/request/vehicle")]
    public class VehicleRequestController : ApiController
    {

        VehicleRequestService vehicleRequestService = new VehicleRequestService();

        [Route("create")]
        [HttpPost]
        public IHttpActionResult createVehicleRequest(VehicleRequest vehicleRequest)
        {
            var newVehicleRequest = vehicleRequestService.createVehicleRequest(vehicleRequest);
            if (!newVehicleRequest.Success)
            {
                return BadRequest(newVehicleRequest.Message);
            }
            return Ok(vehicleRequest);
        }
    }
}