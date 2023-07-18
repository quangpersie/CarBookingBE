using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data;

namespace CarBookingBE.Services
{
    public class RoleService
    {
        MyDbContext _db = new MyDbContext();
        public Result<List<Role>> getAllRole(int page, int limit)
        {
            try
            {
                var rList = _db.Roles.Where(r => r.IsDeleted == false)
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
                if (rList.Any())
                {
                    return new Result<List<Role>>(true, "Get all roles successfully !", rList);
                }
                return new Result<List<Role>>(false, "There's no data !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<Role>>(false, "Internal error !");
            }
        }

        public Result<Role> getRoleById(string id)
        {
            try
            {
                var rId = Guid.Parse(id);
                var gRole = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Id == rId);
                if (gRole != null)
                {
                    return new Result<Role>(true, "Get role successfully !", gRole);
                }
                return new Result<Role>(false, "Data does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }

        public Result<Role> addRole(Role role)
        {
            try
            {
                if (role == null || role.Title == null)
                {
                    return new Result<Role>(false, "Missing parameter !");
                }
                var reusable = _db.Roles.Where(r => r.IsDeleted == true && r.Title == role.Title).FirstOrDefault();
                if (reusable != null)
                {
                    reusable.IsDeleted = false;
                    _db.SaveChanges();
                    return new Result<Role>(true, "Add role for user successfully !", reusable);
                }

                var checkExist = _db.Roles.FirstOrDefault(r => r.Title == role.Title);
                if (checkExist == null)
                {
                    _db.Roles.Add(role);
                    _db.SaveChanges();
                    return new Result<Role>(true, "Add role successfully !", role);
                }
                return new Result<Role>(false, "This title of role has already existed !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }

        public Result<Role> editRole(string id, Role role)
        {
            try
            {
                if (role == null || role.Title == null)
                {
                    return new Result<Role>(false, "Missing parameter !");
                }
                var rId = Guid.Parse(id);
                var eRole = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Id == rId);
                if(eRole != null)
                {
                    eRole.Title = role.Title;
                    _db.SaveChanges();
                    return new Result<Role>(true, "Edit role title successfully !", eRole);
                }
                return new Result<Role>(false, "Data does not exist");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }

        public Result<Role> deleteRole(string id)
        {
            try
            {
                var rId = Guid.Parse(id);
                var dRole = _db.Roles.FirstOrDefault(r => r.IsDeleted == false && r.Id == rId);
                if (dRole != null)
                {
                    var deleteUserRole = _db.UserRoles.Where(ur => ur.IsDeleted == false && ur.RoleId == rId).ToList();
                    foreach (var item in deleteUserRole)
                    {
                        item.IsDeleted = true;
                    }
                    dRole.IsDeleted = true;
                    _db.SaveChanges();
                    return new Result<Role>(true, "Delete role successfully !");
                }
                return new Result<Role>(false, "Data does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Role>(false, "Internal error !");
            }
        }
    }
}