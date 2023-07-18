using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CarBookingBE.Services
{
    public class DepartmentService
    {
        MyDbContext _db = new MyDbContext();
        UtilMethods util = new UtilMethods();
        public Result<List<Department>> getAll(int page, int limit)
        {
            try
            {
                var departments = _db.Departments.Where(d => d.IsDeleted == false)
                .OrderByDescending(user => user.Code)
                .Skip(util.getSkip(page, limit))
                .Take(limit)
                .ToList();
                if(departments == null)
                {
                    return new Result<List<Department>>(false, "Get all departments fail !");
                }
                return new Result<List<Department>>(true, "Get all departments successfully !", departments);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return new Result<List<Department>>(false, "Internal error !");
            }
        }
        public Result<Department> addDepartment(Department department)
        {
            try
            {
                if (department == null || department.Name == null || department.ContactInfo == null || department.Code == null || department.Description == null)
                {
                    return new Result<Department>(false, "Missing parameter(s) !");
                }
                _db.Departments.Add(department);
                _db.SaveChanges();
                return new Result<Department>(true, "Add department succesfully !", department);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Department>(false, "Internal error !");
            }
        }
        public Result<Department> getDepartment(string id)
        {
            try
            {
                var department = _db.Departments.Find(Guid.Parse(id));
                if(department == null || department.IsDeleted == true)
                {
                    return new Result<Department>(false, "Department does not exist !");
                }
                return new Result<Department>(true, "Get department successfully !", department);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e);
                return new Result<Department>(false, "Internal error !");
            }
        }
        public Result<Department> editDepartment(string id, Department dUpdate)
        {
            try
            {
                var dTarget = _db.Departments.Find(Guid.Parse(id));
                if(dTarget == null || dTarget.IsDeleted == true)
                {
                    return new Result<Department>(false, "Department do not exist !");
                }
                if(dUpdate.Name != null) dTarget.Name = dUpdate.Name;
                if(dUpdate.ContactInfo != null) dTarget.ContactInfo = dUpdate.ContactInfo;
                if(dUpdate.Code != null) dTarget.Code = dUpdate.Code;
                if(dUpdate.UnderDepartment != null) dTarget.UnderDepartment = dUpdate.UnderDepartment;
                if(dUpdate.Description != null) dTarget.Description = dUpdate.Description;
                _db.SaveChanges();
                return new Result<Department>(true, "Edit department successfully !");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e);
                return new Result<Department>(false, "Internal error !");
            }
        }
        public Result<Department> deleteDepartment(string id)
        {
            try
            {
                var del = _db.Departments.Find(Guid.Parse(id));
                if(del != null || del.IsDeleted == true)
                {
                    del.IsDeleted = true;
                    _db.SaveChanges();
                    return new Result<Department>(false, "Department do not exist !");
                }
                return new Result<Department>(true, "Delete department successfully !");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e);
                return new Result<Department>(false, "Internal error !");
            }
        }
    }
}