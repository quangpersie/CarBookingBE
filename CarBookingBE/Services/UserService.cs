using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CarBookingBE.Services
{
    public class UserService
    {
        MyDbContext _db = new MyDbContext();
        HandlePassword hp = new HandlePassword();
        TokenProps jwt = new TokenProps();
        FileService fileService = new FileService();
        RoleConstants roleConstants = new RoleConstants();
        UserRoleService urService = new UserRoleService();
        UtilMethods util = new UtilMethods();
        public Result<LoginReturnDTO> loginService(LoginDTO user)
        {
            if (user == null || user.Username == null || user.Password == null)
            {
                return new Result<LoginReturnDTO>(false, "Missing required parameter !");
            }
            else if (user.Username.Trim().Length == 0)
            {
                return new Result<LoginReturnDTO>(false, "Username is required !");
            }
            else if (user.Password.Trim().Length == 0)
            {
                return new Result<LoginReturnDTO>(false, "Password is required !");
            }
            else
            {
                string hash = hp.HashPassword(user.Password);
                var userInfo = _db.Users
                    .Where(ur => ur.IsDeleted == false && ur.Username == user.Username && ur.Password == hash)
                    .Select(ur => new AccountLoginReturnDTO
                    {
                        Id = ur.Id,
                        Username = ur.Username,
                        Email = ur.Email,
                        FirstName = ur.FirstName,
                        LastName = ur.LastName,
                        AvatarPath = ur.AvatarPath,
                    })
                    .FirstOrDefault();
                //var validUser = _db.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == hash);
                if (userInfo != null)
                {
                    string secretKey = jwt.secretKey;
                    string issuer = jwt.issuer;
                    string audience = jwt.audience;
                    int expirationMinutes = jwt.expirationMinutes;
                    Claim[] customClaims = new Claim[]
                    {
                        new Claim("CurId", userInfo.Id.ToString()),
                        new Claim("Username", user.Username),
                        new Claim("Password", user.Password)
                    };
                    string jwtToken = jwt.GenerateJwtToken(secretKey, issuer, audience, expirationMinutes, customClaims);
                    //bool check = jwt.VerifyJwtToken(jwtToken, secretKey);
                    var userRole = _db.UserRoles
                        .Where(ur => ur.IsDeleted == false && ur.UserId == userInfo.Id)
                        .Select(ur => new UserRolesDTO
                        {
                            Role = new RoleDTO
                            {
                                Id = ur.Role.Id,
                                Title = ur.Role.Title
                            }
                        })
                        .ToList();

                    return new Result<LoginReturnDTO>(true, "Login successfully !", new LoginReturnDTO(userInfo, jwtToken, userRole));
                }
                else
                {
                    return new Result<LoginReturnDTO>(false, "Username or password is not correct !");
                }
            }
        }
        public Result<Account> registerService(Guid curId, HttpPostedFile file, Account user, string roleId)
        {
            try
            {
                var rId = int.Parse(roleId);
                if (user == null)
                {
                    return new Result<Account>(false, "Missing all parameters !");
                }
                else if (user.Email == null || user.Email.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Email is required !");
                }
                else if (_db.Users.FirstOrDefault(u => u.Email == user.Email) != null)
                {
                    return new Result<Account>(false, "This email has existed !");
                }
                else if (user.Password == null || user.Password.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Password is required !");
                }
                else if (user.FirstName == null || user.FirstName.Trim().Length == 0)
                {
                    return new Result<Account>(false, "First name is required !");
                }
                else if (user.LastName == null || user.LastName.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Last name is required !");
                }
                else if (user.Sex.ToString() == null || user.Sex.ToString().Trim().Length == 0)
                {
                    return new Result<Account>(false, "Sex is required !");
                }
                else if (user.EmployeeNumber == null || user.EmployeeNumber.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Employee number is required !");
                }
                else if (_db.Users.FirstOrDefault(u => u.EmployeeNumber == user.EmployeeNumber) != null)
                {
                    return new Result<Account>(false, "This employee number has existed !");
                }
                else if (roleId == null || roleId.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Missing role id of user !");
                }
                else if (_db.Roles.Where(role => role.IsDeleted == false && role.Id == rId).FirstOrDefault() == null)
                {
                    return new Result<Account>(false, "Role with input id does not exist !");
                }
                else
                {
                    string flag = "";
                    if (file != null && file.ContentType != null && file.FileName.Count() != 0)
                    {
                        var uploadResult = fileService.uploadAvatar(curId, file);
                        if (!uploadResult.Success)
                        {
                            return new Result<Account>(false, uploadResult.Message);
                        }
                        flag = uploadResult.Data;
                    }
                    Account newUser = new Account();
                    newUser.Email = user.Email;
                    newUser.Username = user.Email;
                    newUser.Password = hp.HashPassword(user.Password);
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Sex = user.Sex;
                    newUser.EmployeeNumber = user.EmployeeNumber;
                    newUser.IsDeleted = false;
                    newUser.Created = DateTime.Now;
                    if (flag.Length > 0)
                    {
                        newUser.AvatarPath = flag;
                    }
                    else
                    {
                        newUser.AvatarPath = "Files/default-user-profile.png";
                    }
                    _db.Users.Add(newUser);

                    AccountRole ar = new AccountRole
                    {
                        UserId = newUser.Id,
                        RoleId = rId
                    };
                    _db.SaveChanges();
                    var addUserRoleResult = urService.addUserRole(ar);
                    if (!addUserRoleResult.Success)
                    {
                        return new Result<Account>(false, addUserRoleResult.Message);
                    }

                    return new Result<Account>(true, "Register new user successfully !", newUser);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Account>(false, "Internal error !");
            }
        }
        public Result<Pagination<Account>> getUsersService(int page, int limit)
        {
            try
            {
                var users = _db.Users.Where(u => u.IsDeleted == false)
                .OrderByDescending(user => user.Created)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

                var usersPagination = new Pagination<Account> { 
                    PerPage = limit,
                    CurrentPage = page,
                    TotalPage = (users.Count + limit - 1) / limit,
                    ListData = users
                };
                if (users == null)
                {
                    return new Result<Pagination<Account>>(true, "There's no data !", new Pagination<Account>());
                }
                return new Result<Pagination<Account>>(true, "Get all users successfully !", usersPagination);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Pagination<Account>>(false, "Internal error !");
            }
        }

        public Result<Account> getProfileService(string id)
        {
            try
            {
                var user = _db.Users.Find(Guid.Parse(id));
                if (user == null || user.IsDeleted == true)
                {
                    return new Result<Account>(false, "User does not exist");
                }
                return new Result<Account>(true, "Get user profile successfully !", user);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Account>(false, "Internal error !");
            }
        }

        public Result<Account> editProfileService(Guid curId, HttpPostedFile postedFile, string userIdString, NameValueCollection updateUser)
        {
            try
            {
                var userId = Guid.Parse(userIdString);
                var user = _db.Users.Find(userId);
                if (user == null || user.IsDeleted == true)
                {
                    return new Result<Account>(false, "User do not exist !");
                }

                //start edit fields
                if (postedFile != null)
                {
                    var rs = fileService.uploadAvatar(curId, postedFile);
                    if (!rs.Success)
                    {
                        return new Result<Account>(false, rs.Message);
                    }
                    user.AvatarPath = rs.Data;
                }

                bool isAdmin = util.userHasRole(curId, roleConstants.ADMIN);
                if (isAdmin)
                {
                    //Password, IsDeleted
                    if (updateUser["Birthday"].Trim() != null)
                    {
                        try
                        {
                            user.Birthday = DateTime.ParseExact(updateUser["Birthday"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (user.Birthday > DateTime.Now)
                            {
                                return new Result<Account>(false, "Birthday cannot exceed time in the future");
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Birthday");
                        }
                    }
                    if (updateUser["StartingDateOfficial"].Trim() != null)
                    {
                        try
                        {
                            user.StartingDateOfficial = DateTime.ParseExact(updateUser["StartingDateOfficial"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Starting Date Official");
                        }
                    }
                    if (updateUser["LeavingDate"].Trim() != null)
                    {
                        try
                        {
                            user.LeavingDate = DateTime.ParseExact(updateUser["LeavingDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Leaving Date");
                        }
                    }
                    if (updateUser["StartDateMaternityLeave"].Trim() != null)
                    {
                        try
                        {
                            user.StartDateMaternityLeave = DateTime.ParseExact(updateUser["StartDateMaternityLeave"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Start Date Maternity Leave");
                        }
                    }

                    if (updateUser["Username"].Trim() != null)
                    {
                        if (_db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Username == updateUser["Username"]) != null)
                        {
                            return new Result<Account>(false, "This username's already existed !");
                        }
                        user.Username = updateUser["Username"];
                    }
                    if (updateUser["Email"].Trim() != null)
                    {
                        if (!util.IsValidEmail(updateUser["Email"]))
                        {
                            return new Result<Account>(false, "Wrong format of email !");
                        }
                        user.Email = updateUser["Email"];
                    }
                    if (updateUser["EmployeeNumber"].Trim() != null)
                    {
                        if (_db.Users.FirstOrDefault(u => u.IsDeleted == false && u.EmployeeNumber == updateUser["EmployeeNumber"]) != null)
                        {
                            return new Result<Account>(false, "Duplicate employee number !");
                        }
                        user.EmployeeNumber = updateUser["EmployeeNumber"];
                    }
                    if (updateUser["AvatarPath"].Trim() != null)
                    {
                        user.AvatarPath = updateUser["AvatarPath"];
                    }
                    if (updateUser["FirstName"].Trim() != null) user.FirstName = updateUser["FirstName"];
                    if (updateUser["LastName"].Trim() != null) user.LastName = updateUser["LastName"];
                    if (updateUser["Sex"].Trim() != null)
                    {
                        try
                        {
                            user.Sex = bool.Parse(updateUser["Sex"]);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Type of sex is invalid !");
                        }
                    }
                    if (updateUser["JobTitle"].Trim() != null) user.JobTitle = updateUser["JobTitle"];
                    if (updateUser["Company"].Trim() != null) user.Company = updateUser["Company"];
                    if (updateUser["Unit"].Trim() != null) user.Unit = updateUser["Unit"];
                    if (updateUser["Function"].Trim() != null) user.Function = updateUser["Function"];
                    if (updateUser["SectionsOrTeam"].Trim() != null) user.SectionsOrTeam = updateUser["SectionsOrTeam"];
                    if (updateUser["Groups"].Trim() != null) user.Groups = updateUser["Groups"];
                    if (updateUser["OfficeLocation"].Trim() != null) user.OfficeLocation = updateUser["OfficeLocation"];
                    if (updateUser["LineManager"].Trim() != null) user.LineManager = updateUser["LineManager"];
                    if (updateUser["BelongToDepartments"].Trim() != null) user.BelongToDepartments = updateUser["BelongToDepartments"];

                }
                // user can edit
                if (updateUser["DateO.Trim()fIdCard"] != null)
                {
                    try
                    {
                        user.DateOfIdCard = DateTime.ParseExact(updateUser["DateOfIdCard"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (user.DateOfIdCard > DateTime.Now)
                        {
                            return new Result<Account>(false, "Date Of Id Card cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Date Of Id Card");
                    }
                }
                
                if (updateUser["Start.Trim()ingDate"] != null)
                {
                    try
                    {
                        user.StartingDate = DateTime.ParseExact(updateUser["StartingDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (user.StartingDate > DateTime.Now)
                        {
                            return new Result<Account>(false, "Starting Date (Interviewed date) cannot exceed time in the future");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Starting Date (Interviewed date)");
                    }
                }
                if (updateUser["Rank"].Trim() != null) user.Rank = updateUser["Rank"];
                if (updateUser["EmployeeType"].Trim() != null) user.EmployeeType = updateUser["EmployeeType"];
                if (updateUser["Rights"].Trim() != null) user.Rights = updateUser["Rights"];
                if (updateUser["Nation"].Trim() != null) user.Nation = updateUser["Nation"];
                if (updateUser["Phone"].Trim() != null) user.Phone = updateUser["Phone"];
                if (updateUser["IdCardNumber"].Trim() != null) user.IdCardNumber = updateUser["IdCardNumber"];
                if (updateUser["PlaceOfIdCard"].Trim() != null) user.PlaceOfIdCard = updateUser["PlaceOfIdCard"];
                if (updateUser["HealthInsurance"].Trim() != null) user.HealthInsurance = updateUser["HealthInsurance"];
                if (updateUser["Note"].Trim() != null) user.Note = updateUser["Note"];
                if (updateUser["AcademicLevel"].Trim() != null) user.AcademicLevel = updateUser["AcademicLevel"];
                if (updateUser["Qualification"].Trim() != null) user.Qualification = updateUser["Qualification"];
                if (updateUser["BusinessPhone"].Trim() != null) user.BusinessPhone = updateUser["BusinessPhone"];
                if (updateUser["HomePhone"].Trim() != null) user.HomePhone = updateUser["HomePhone"];
                if (updateUser["PersonalEmail"].Trim() != null) user.PersonalEmail = updateUser["PersonalEmail"];
                if (updateUser["BankName"].Trim() != null) user.BankName = updateUser["BankName"];
                if (updateUser["BankBranchNumber"].Trim() != null) user.BankBranchNumber = updateUser["BankBranchNumber"];
                if (updateUser["BankBranchName"].Trim() != null) user.BankBranchName = updateUser["BankBranchName"];
                if (updateUser["BankAccountNumber"].Trim() != null) user.BankAccountNumber = updateUser["BankAccountNumber"];
                if (updateUser["BankAccountName"].Trim() != null) user.BankAccountName = updateUser["BankAccountName"];
                if (updateUser["Street"].Trim() != null) user.Street = updateUser["Street"];
                if (updateUser["FlatNumber"].Trim() != null) user.FlatNumber = updateUser["FlatNumber"];
                if (updateUser["City"].Trim() != null) user.City = updateUser["City"];
                if (updateUser["Province"].Trim() != null) user.Province = updateUser["Province"];
                if (updateUser["PostalCode"].Trim() != null) user.PostalCode = updateUser["PostalCode"];
                if (updateUser["Country"].Trim() != null) user.Country = updateUser["Country"];
                if (updateUser["MartialStatus"].Trim() != null) user.MartialStatus = updateUser["MartialStatus"];
                if (updateUser["ContactName"].Trim() != null) user.ContactName = updateUser["ContactName"];
                if (updateUser["Relationship"].Trim() != null) user.Relationship = updateUser["Relationship"];
                if (updateUser["PhoneR"].Trim() != null) user.PhoneR = updateUser["PhoneR"];
                if (updateUser["StreetR"].Trim() != null) user.StreetR = updateUser["StreetR"];
                if (updateUser["FlatNumberR"].Trim() != null) user.FlatNumberR = updateUser["FlatNumberR"];
                if (updateUser["CityR"].Trim() != null) user.CityR = updateUser["CityR"];
                if (updateUser["ProvinceR"].Trim() != null) user.ProvinceR = updateUser["ProvinceR"];
                if (updateUser["PostalCodeR"].Trim() != null) user.PostalCodeR = updateUser["PostalCodeR"];
                if (updateUser["CountryR"].Trim() != null) user.CountryR = updateUser["CountryR"];
                user.Created = DateTime.Now;

                _db.SaveChanges();
                return new Result<Account>(true, "Edit profile successfully !", user);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return new Result<Account>(false, "Internal error !");
            }
        }
    }
}