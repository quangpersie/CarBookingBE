using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace CarBookingBE.Services
{
    public class DepartmentMemberService
    {
        MyDbContext _db = new MyDbContext();
        public Result<List<DepartmentMemberDTO>> getAll(int page, int limit)
        {
            try
            {
                var dmsList = _db.DepartmentsMembers.Include(d => d.User).Include(d => d.Department)
                    .Where(dm => dm.IsDeleted == false)
                    .Select(d => new DepartmentMemberDTO
                    {
                        Id = d.Id,
                        Position = d.Position,
                        User = new AccountDTO
                        {
                            Id = d.User.Id,
                            Username = d.User.Username,
                            FirstName = d.User.FirstName,
                            LastName = d.User.LastName,
                            JobTitle = d.User.JobTitle
                        },
                        Department = new DepartmentDTO
                        {
                            Id = d.Department.Id,
                            Name = d.Department.Name
                        }
                    })
                    .OrderBy(dm => dm.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();
                if (!dmsList.Any())
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "There's no data !");
                }
                return new Result<List<DepartmentMemberDTO>>(true, "Get all data successfully !", dmsList);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<DepartmentMemberDTO>>(false, "Internal error !");
            }
        }
        public Result<List<DepartmentMemberDTO>> getByDepartmentId(string departmentId)
        {
            try
            {
                if(departmentId == null)
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "Missing id of department !");
                }
                var dId = Guid.Parse(departmentId);
                var pList = _db.DepartmentsMembers
                    .Include(d => d.User).Include(d => d.Department)
                    .Where(d => d.IsDeleted == false)
                    .Select(d => new DepartmentMemberDTO
                    {
                        Id = d.Id,
                        Position = d.Position,
                        User = new AccountDTO
                        {
                            Id = d.User.Id,
                            Username = d.User.Username,
                            Email = d.User.Email,
                            FirstName = d.User.FirstName,
                            LastName = d.User.LastName,
                            JobTitle = d.User.JobTitle
                        },
                        Department = new DepartmentDTO
                        {
                            Id = d.Department.Id,
                            Name = d.Department.Name
                        }
                    })
                    .Where(d => d.Department.Id == dId)
                    .ToList();

                if (!pList.Any())
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "There's no data with this department !");
                }
                return new Result<List<DepartmentMemberDTO>>(true, $"Get all data in the department successfully)", pList);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<DepartmentMemberDTO>>(false, "Internal error !");
            }
        }
        public Result<List<DepartmentMemberDTO>> getByPositionWithDeparmentId(string departmentId, string position)
        {
            try
            {
                if (departmentId == null)
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "Missing id of department !");
                }
                else if(position == null)
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "Missing position field !");
                }
                var dId = Guid.Parse(departmentId);
                var pList = _db.DepartmentsMembers
                    .Include(d => d.User).Include(d => d.Department)
                    .Where(d => d.IsDeleted == false && d.Position.Equals(position))
                    .Select(d => new DepartmentMemberDTO
                    {
                        Id = d.Id,
                        Position = d.Position,
                        User = new AccountDTO
                        {
                            Id = d.User.Id,
                            Username = d.User.Username,
                            Email = d.User.Email,
                            FirstName = d.User.FirstName,
                            LastName = d.User.LastName,
                            JobTitle = d.User.JobTitle
                        },
                        Department = new DepartmentDTO
                        {
                            Id = d.Department.Id,
                            Name = d.Department.Name
                        }
                    })
                    .Where(d => d.Department.Id == dId)
                    .ToList();

                if (!pList.Any())
                {
                    return new Result<List<DepartmentMemberDTO>>(false, "There's no data with this position !");
                }
                return new Result<List<DepartmentMemberDTO>>(true, $"Get all data in the department successfully (position = {position.ToLower()})", pList);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<DepartmentMemberDTO>>(false, "Internal error !");
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
                var reusable = _db.DepartmentsMembers
                    .Where(d => d.IsDeleted == true && d.UserId == newDM.UserId &&
                    d.DepartmentId == newDM.DepartmentId && d.Position == newDM.Position)
                    .FirstOrDefault();
                if (reusable != null)
                {
                    reusable.IsDeleted = false;
                    _db.SaveChanges();
                    return new Result<DepartmentMember>(true, "Add role for user successfully !", reusable);
                }
                var checkExist = _db.DepartmentsMembers
                    .Where(d => d.IsDeleted == false && d.UserId == newDM.UserId && d.DepartmentId == newDM.DepartmentId)
                    .FirstOrDefault();
                if (checkExist != null)
                {
                    return new Result<DepartmentMember>(false, "This employee's already existed in the department !");
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
                return new Result<DepartmentMember>(true, "Get data successfully !", dm);
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
                    return new Result<DepartmentMember>(false, "Data (with input id) does not exist !");
                }
                del.IsDeleted = true;
                _db.SaveChanges();
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