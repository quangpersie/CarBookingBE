using CarBookingBE.Utils;
using CarBookingTest.Models;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace CarBookingBE.Services
{
    public class DepartmentMemberService
    {
        MyDbContext _db = new MyDbContext();
        UtilMethods util = new UtilMethods();
        public Result<List<DepartmentMember>> getAll(int page, int limit)
        {
            try
            {
                var dmsList = _db.DepartmentsMembers.Where(dm => dm.IsDeleted == false)
                    .OrderByDescending(dm => dm.Department)
                    .Skip(util.getSkip(page, limit))
                    .Take(limit)
                    .ToList();
                if (dmsList == null)
                {
                    return new Result<List<DepartmentMember>>(false, "There's no data !");
                }
                return new Result<List<DepartmentMember>>(true, "Get all datas successfully !", dmsList);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<DepartmentMember>>(false, "Internal error !");
            }
        }
        public Result<DepartmentMember> addDepartmentMember(DepartmentMember newDM)
        {
            try
            {
                if (newDM == null || newDM.UserId == null || newDM.DepartmentId == null || newDM.Position == null)
                {
                    return new Result<DepartmentMember>(false, "Missing parameter(s) !");
                }
                _db.DepartmentsMembers.Add(newDM);
                _db.SaveChanges();
                return new Result<DepartmentMember>(true, "Add data successfully !", newDM);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
        public Result<DepartmentMember> getDepartmentMember(string id)
        {
            try
            {
                var dm = _db.DepartmentsMembers.Find(Guid.Parse(id));
                if (dm == null || dm.IsDeleted == true)
                {
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                return new Result<DepartmentMember>(false, "Get data successfully !", dm);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
        public Result<DepartmentMember> editDepartmentMember(string id, DepartmentMember dmUpdate)
        {
            try
            {
                var dmTarget = _db.DepartmentsMembers.Find(Guid.Parse(id));
                if (dmTarget == null || dmTarget.IsDeleted == true)
                {
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                if (dmUpdate.UserId != null) dmUpdate.UserId = dmUpdate.UserId;
                if (dmUpdate.DepartmentId != null) dmUpdate.DepartmentId = dmUpdate.DepartmentId;
                if (dmUpdate.Position != null) dmUpdate.Position = dmUpdate.Position;
                _db.SaveChanges();
                return new Result<DepartmentMember>(true, "Edit data successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
        public Result<DepartmentMember> deleteDepartmentMember(string id)
        {
            try
            {
                var del = _db.DepartmentsMembers.Find(Guid.Parse(id));
                if (del != null || del.IsDeleted == true)
                {
                    del.IsDeleted = true;
                    _db.SaveChanges();
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                return new Result<DepartmentMember>(true, "Delete data successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Result<DepartmentMember>(false, "Internal error !");
            }
        }
    }
}