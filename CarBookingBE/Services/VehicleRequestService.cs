using CarBookingBE.DTOs;
using CarBookingBE.Models;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.Services
{
    public class VehicleRequestService
    {
        MyDbContext db = new MyDbContext();
        UtilMethods utilMethods = new UtilMethods();
        public Result<VehicleRequest> createVehicleRequest(VehicleRequest vehicleRequest)
        {
            var isAuthorized = utilMethods.isAuthorized(new RoleConstants(true, true, false, false, false));
            if (isAuthorized.Success)
            {
                if (vehicleRequest.RequestId == null || vehicleRequest.RotaionId == null || vehicleRequest.DriverId == null || vehicleRequest.DriverMobile == null || vehicleRequest.DriverCarplate == null || vehicleRequest.Reason == null)
                {
                    return new Result<VehicleRequest>(false, "Miss Parameters");
                }

                Request request = db.Requests.SingleOrDefault(r => r.IsDeleted == false
                    && r.Id == vehicleRequest.RequestId && r.Status == "Approved");
                if (request == null)
                {
                    return new Result<VehicleRequest>(false, "Request Not Found");
                }

                /*var user = db.DepartmentsMembers.SingleOrDefault(dm => dm.IsDeleted == false && dm.UserId == vehicleRequest.DriverId && dm.DepartmentId == );*/
                var user = db.Users.SingleOrDefault(u => u.IsDeleted == false && u.Id == vehicleRequest.DriverId);
                if (user == null)
                {
                    return new Result<VehicleRequest>(false, "User Not Found");
                }

                var rotaion = db.Rotations.SingleOrDefault(r => r.IsDeleted == false && r.Id == vehicleRequest.RotaionId);
                if (rotaion == null)
                {
                    return new Result<VehicleRequest>(false, "Rotaion Not Found");
                }

                request.Status = "Done";
                db.VehicleRequests.Add(vehicleRequest);
            }
            else
            {
                return new Result<VehicleRequest>(false, "User Permission Failed!");
            }

            db.SaveChanges();
            return new Result<VehicleRequest>(true, "Create Vehicle Request Success", vehicleRequest);
        }

        public Result<List<Rotation>> getAllRotation()
        {
            var rotations = db.Rotations.Where(r => r.IsDeleted == false).OrderBy(r => r.Type).ToList();
            return new Result<List<Rotation>>(true, "Get Success", rotations);
        }

    }
}