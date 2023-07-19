using CarBookingBE.Utils;
using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CarBookingBE.Services
{
    public class UserRoleService
    {
        MyDbContext _db = new MyDbContext();

        public Result<List<AccountRole>> getAllUserRole(int page, int limit)
        {
            try
            {
                var urList = _db.UserRoles
                .Where(r => r.IsDeleted == false)
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
                if (urList.Any())
                {
                    return new Result<List<AccountRole>>(true, "Get all roles successfully !", urList);
                }
                return new Result<List<AccountRole>>(false, "There's no data !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<List<AccountRole>>(false, "Internal error !");
            }
        }

        public Result<AccountRole> getUserRoleById(AccountRole accRole)
        {
            try
            {
                if (accRole == null || accRole.Id == null)
                {
                    return new Result<AccountRole>(false, "Missing or Invalid id field !");
                }
                var arId = accRole.Id;
                var existedUR = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.Id == arId);
                if (existedUR != null)
                {
                    return new Result<AccountRole>(true, "Get data by id successfully !", existedUR);
                }
                return new Result<AccountRole>(false, "There's no data !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }
        public Result<AccountRole> addUserRole(AccountRole accRole)
        {
            try
            {
                if(accRole == null || accRole.UserId == null || accRole.RoleId == null)
                {
                    return new Result<AccountRole>(false, "Missing parameter(s) or Invalid id field !");
                }
                var uId = accRole.UserId;
                var rId = accRole.RoleId;
                var user = _db.Users.Find(uId);
                var role = _db.Roles.Find(rId);
                if (user != null && role != null)
                {
                    var reusable = _db.UserRoles.Where(r => r.IsDeleted == true && r.UserId == uId && r.RoleId == rId).FirstOrDefault();
                    if(reusable != null)
                    {
                        reusable.IsDeleted = false;
                        _db.SaveChanges();
                        return new Result<AccountRole>(true, "Add role for user successfully !", reusable);
                    }

                    var isExist = _db.UserRoles.Where(r => r.IsDeleted == false && r.UserId == uId && r.RoleId == rId).FirstOrDefault();
                    if (isExist == null)
                    {
                        var newUserRole = new AccountRole { UserId = uId, RoleId = rId };
                        _db.UserRoles.Add(newUserRole);
                        _db.SaveChanges();
                        return new Result<AccountRole>(true, "Add role for user successfully !", newUserRole);
                    }
                    return new Result<AccountRole>(false, "Add fail, this user's already had the input role !");
                }
                return new Result<AccountRole>(false, "User or role do not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }

        public Result<AccountRole> editUserRole(AccountRole accRole)
        {
            try
            {
                if (accRole == null || accRole.Id == null || accRole.UserId == null || accRole.RoleId == null)
                {
                    return new Result<AccountRole>(false, "Missing parameter(s) or Invalid id field !");
                }
                var urId = accRole.Id;
                var uId = accRole.UserId;
                var rId = accRole.RoleId;
                var userRole = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.Id == urId);
                if (userRole == null)
                {
                    return new Result<AccountRole>(false, "Data does not exist !");
                }
                var user = _db.Users.Find(uId);
                var role = _db.Roles.Find(rId);
                if (user != null && role != null)
                {
                    var existedUR = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.UserId == uId && ur.RoleId == rId);
                    if (existedUR != null)
                    {
                        return new Result<AccountRole>(false, "Edit fail, this user's already had the input role !");
                    }
                    userRole.UserId = uId;
                    userRole.RoleId = rId;
                    _db.SaveChanges();
                    return new Result<AccountRole>(true, "Edit role for user successfully !", existedUR);
                }
                return new Result<AccountRole>(false, "User or role do not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }

        public Result<AccountRole> deleteUserRole(AccountRole accRole)
        {
            try
            {
                if (accRole == null || accRole.Id == null)
                {
                    return new Result<AccountRole>(false, "Missing or Invalid id field !");
                }
                var arId = accRole.Id;
                var userRole = _db.UserRoles.FirstOrDefault(ur => ur.IsDeleted == false && ur.Id == arId);
                if (userRole == null)
                {
                    return new Result<AccountRole>(false, "Input data does not exist !");
                }
                userRole.IsDeleted = true;
                _db.SaveChanges();
                return new Result<AccountRole>(true, "Delete role for user successfully !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<AccountRole>(false, "Internal error !");
            }
        }
    }
}