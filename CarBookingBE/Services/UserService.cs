using CarBookingBE.DTOs;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
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
        DepartmentMemberService dms = new DepartmentMemberService();
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
                        var uploadResult = fileService.uploadAvatar(curId.ToString(), file);
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
                        newUser.AvatarPath = "Files/Avatar/default-user-profile.png";
                    }
                    _db.Users.Add(newUser);

                    AccountRole ar = new AccountRole
                    {
                        UserId = user.Id,
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
                var userId = Guid.Parse(id);
                var user = _db.Users.Include(u => u.UserRoles)
                    .FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
                if (user == null)
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

        public Result<Account> editProfileWithPostFile(Guid curId, HttpPostedFile postedFile, string userIdString, NameValueCollection updateUser)
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
                    var rs = fileService.uploadAvatar(curId.ToString(), postedFile);
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
                    if (updateUser["Birthday"] != null)
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
                    if (updateUser["StartingDateOfficial"] != null)
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
                    if (updateUser["LeavingDate"] != null)
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
                    if (updateUser["StartDateMaternityLeave"] != null)
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

                    if (updateUser["Username"] != null)
                    {
                        if (_db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Username == updateUser["Username"]) != null)
                        {
                            return new Result<Account>(false, "This username's already existed !");
                        }
                        user.Username = updateUser["Username"];
                    }
                    if (updateUser["Email"] != null)
                    {
                        if (!util.IsValidEmail(updateUser["Email"]))
                        {
                            return new Result<Account>(false, "Wrong format of email !");
                        }
                        user.Email = updateUser["Email"];
                    }
                    if (updateUser["EmployeeNumber"] != null)
                    {
                        if (_db.Users.FirstOrDefault(u => u.IsDeleted == false && u.EmployeeNumber == updateUser["EmployeeNumber"]) != null)
                        {
                            return new Result<Account>(false, "Duplicate employee number !");
                        }
                        user.EmployeeNumber = updateUser["EmployeeNumber"];
                    }
                    if (updateUser["AvatarPath"] != null)
                    {
                        user.AvatarPath = updateUser["AvatarPath"];
                    }
                    if (updateUser["FirstName"] != null) user.FirstName = updateUser["FirstName"];
                    if (updateUser["LastName"] != null) user.LastName = updateUser["LastName"];
                    if (updateUser["Sex"] != null)
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
                    if (updateUser["JobTitle"] != null) user.JobTitle = updateUser["JobTitle"];
                    if (updateUser["Company"] != null) user.Company = updateUser["Company"];
                    if (updateUser["Unit"] != null) user.Unit = updateUser["Unit"];
                    if (updateUser["Function"] != null) user.Function = updateUser["Function"];
                    if (updateUser["SectionsOrTeam"] != null) user.SectionsOrTeam = updateUser["SectionsOrTeam"];
                    if (updateUser["Groups"] != null) user.Groups = updateUser["Groups"];
                    if (updateUser["OfficeLocation"] != null) user.OfficeLocation = updateUser["OfficeLocation"];
                    if (updateUser["LineManager"] != null) user.LineManager = updateUser["LineManager"];
                    if (updateUser["BelongToDepartments"] != null) user.BelongToDepartments = updateUser["BelongToDepartments"];

                }
                // user can edit
                if (updateUser["DateOfIdCard"] != null)
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

                if (updateUser["StartingDate"] != null)
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
                if (updateUser["Rank"] != null) user.Rank = updateUser["Rank"];
                if (updateUser["EmployeeType"] != null) user.EmployeeType = updateUser["EmployeeType"];
                if (updateUser["Rights"] != null) user.Rights = updateUser["Rights"];
                if (updateUser["Nation"] != null) user.Nation = updateUser["Nation"];
                if (updateUser["Phone"] != null) user.Phone = updateUser["Phone"];
                if (updateUser["IdCardNumber"] != null) user.IdCardNumber = updateUser["IdCardNumber"];
                if (updateUser["PlaceOfIdCard"] != null) user.PlaceOfIdCard = updateUser["PlaceOfIdCard"];
                if (updateUser["HealthInsurance"] != null) user.HealthInsurance = updateUser["HealthInsurance"];
                if (updateUser["Note"] != null) user.Note = updateUser["Note"];
                if (updateUser["AcademicLevel"] != null) user.AcademicLevel = updateUser["AcademicLevel"];
                if (updateUser["Qualification"] != null) user.Qualification = updateUser["Qualification"];
                if (updateUser["BusinessPhone"] != null) user.BusinessPhone = updateUser["BusinessPhone"];
                if (updateUser["HomePhone"] != null) user.HomePhone = updateUser["HomePhone"];
                if (updateUser["PersonalEmail"] != null) user.PersonalEmail = updateUser["PersonalEmail"];
                if (updateUser["BankName"] != null) user.BankName = updateUser["BankName"];
                if (updateUser["BankBranchNumber"] != null) user.BankBranchNumber = updateUser["BankBranchNumber"];
                if (updateUser["BankBranchName"] != null) user.BankBranchName = updateUser["BankBranchName"];
                if (updateUser["BankAccountNumber"] != null) user.BankAccountNumber = updateUser["BankAccountNumber"];
                if (updateUser["BankAccountName"] != null) user.BankAccountName = updateUser["BankAccountName"];
                if (updateUser["Street"] != null) user.Street = updateUser["Street"];
                if (updateUser["FlatNumber"] != null) user.FlatNumber = updateUser["FlatNumber"];
                if (updateUser["City"] != null) user.City = updateUser["City"];
                if (updateUser["Province"] != null) user.Province = updateUser["Province"];
                if (updateUser["PostalCode"] != null) user.PostalCode = updateUser["PostalCode"];
                if (updateUser["Country"] != null) user.Country = updateUser["Country"];
                if (updateUser["MartialStatus"] != null) user.MartialStatus = updateUser["MartialStatus"];
                if (updateUser["ContactName"] != null) user.ContactName = updateUser["ContactName"];
                if (updateUser["Relationship"] != null) user.Relationship = updateUser["Relationship"];
                if (updateUser["PhoneR"] != null) user.PhoneR = updateUser["PhoneR"];
                if (updateUser["StreetR"] != null) user.StreetR = updateUser["StreetR"];
                if (updateUser["FlatNumberR"] != null) user.FlatNumberR = updateUser["FlatNumberR"];
                if (updateUser["CityR"] != null) user.CityR = updateUser["CityR"];
                if (updateUser["ProvinceR"] != null) user.ProvinceR = updateUser["ProvinceR"];
                if (updateUser["PostalCodeR"] != null) user.PostalCodeR = updateUser["PostalCodeR"];
                if (updateUser["CountryR"] != null) user.CountryR = updateUser["CountryR"];
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

        //admin page
        public Result<Account> addUserService(AccountForAddDTO user)
        {
            try
            {
                if (user == null)
                {
                    return new Result<Account>(false, "Missing all parameters !");
                }
                else if (user.Roles == null || user.Roles.Length == 0)
                {
                    return new Result<Account>(false, "Empty list roles of user !");
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
                else
                {
                    Account newUser = new Account();
                    newUser.Email = user.Email;
                    newUser.Username = user.Email;
                    newUser.Password = hp.HashPassword(user.Password);
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Sex = user.Sex;
                    newUser.EmployeeNumber = user.EmployeeNumber;
                    //Hard code
                    newUser.IsDeleted = false;
                    newUser.Company = "Opus Solution";
                    newUser.OfficeLocation = "366 Nguyen Trai, Ward 8, District 5, Ho Chi Minh City";
                    newUser.Created = DateTime.Now;
                    //upload client side
                    if (user.AvatarPath != null && util.stringValid(user.AvatarPath))
                    {
                        newUser.AvatarPath = user.AvatarPath;
                    }
                    else
                    {
                        newUser.AvatarPath = "Files/Avatar/default-user-profile.png";
                    }
                    //Optional
                    if (user.Birthday != null)
                    {
                        try
                        {
                            newUser.Birthday = user.Birthday;
                            //user.Birthday = DateTime.ParseExact(newUser.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (newUser.Birthday > DateTime.Now)
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
                    if (user.StartingDateOfficial != null)
                    {
                        try
                        {
                            newUser.StartingDateOfficial = user.StartingDateOfficial;
                            //user.StartingDateOfficial = DateTime.ParseExact(newUser.StartingDateOfficial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Starting Date Official");
                        }
                    }
                    if (user.LeavingDate != null)
                    {
                        try
                        {
                            newUser.LeavingDate = user.LeavingDate;
                            //newUser.LeavingDate = DateTime.ParseExact(newUser.LeavingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Leaving Date");
                        }
                    }
                    if (user.StartDateMaternityLeave != null)
                    {
                        try
                        {
                            newUser.StartDateMaternityLeave = user.StartDateMaternityLeave;
                            //newUser.StartDateMaternityLeave = DateTime.ParseExact(newUser.StartDateMaternityLeave, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e.Message);
                            return new Result<Account>(false, "Wrong format of Start Date Maternity Leave");
                        }
                    }

                    if (user.JobTitle != null && util.stringValid(user.JobTitle)) newUser.JobTitle = user.JobTitle;
                    if (user.Unit != null && util.stringValid(user.Unit)) newUser.Unit = user.Unit;
                    if (user.Function != null && util.stringValid(user.Function)) newUser.Function = user.Function;
                    if (user.SectionsOrTeam != null && util.stringValid(user.SectionsOrTeam)) newUser.SectionsOrTeam = user.SectionsOrTeam;
                    if (user.Groups != null && util.stringValid(user.Groups)) newUser.Groups = user.Groups;
                    if (user.LineManager != null && util.stringValid(user.LineManager)) newUser.LineManager = user.LineManager;
                    if (user.BelongToDepartments != null && util.stringValid(user.BelongToDepartments)) newUser.BelongToDepartments = user.BelongToDepartments;

                    if (user.DateOfIdCard != null)
                    {
                        try
                        {
                            newUser.DateOfIdCard = user.DateOfIdCard;
                            //newUser.DateOfIdCard = DateTime.ParseExact(newUser.DateOfIdCard, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (newUser.DateOfIdCard > DateTime.Now)
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

                    if (user.StartingDate != null)
                    {
                        try
                        {
                            newUser.StartingDate = user.StartingDate;
                            //newUser.StartingDate = DateTime.ParseExact(newUser.StartingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (newUser.StartingDate > DateTime.Now)
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
                    if (user.Rank != null && util.stringValid(user.Rank)) newUser.Rank = user.Rank;
                    if (user.EmployeeType != null && util.stringValid(user.EmployeeType)) newUser.EmployeeType = user.EmployeeType;
                    if (user.Rights != null && util.stringValid(user.Rights)) newUser.Rights = user.Rights;
                    if (user.Nation != null && util.stringValid(user.Nation)) newUser.Nation = user.Nation;
                    if (user.Phone != null && util.stringValid(user.Phone)) newUser.Phone = user.Phone;
                    if (user.IdCardNumber != null && util.stringValid(user.IdCardNumber)) newUser.IdCardNumber = user.IdCardNumber;
                    if (user.PlaceOfIdCard != null && util.stringValid(user.PlaceOfIdCard)) newUser.PlaceOfIdCard = user.PlaceOfIdCard;
                    if (user.HealthInsurance != null && util.stringValid(user.HealthInsurance)) newUser.HealthInsurance = user.HealthInsurance;
                    if (user.Note != null && util.stringValid(user.Note)) newUser.Note = user.Note;
                    if (user.AcademicLevel != null && util.stringValid(user.AcademicLevel)) newUser.AcademicLevel = user.AcademicLevel;
                    if (user.Qualification != null && util.stringValid(user.Qualification)) newUser.Qualification = user.Qualification;
                    if (user.BusinessPhone != null && util.stringValid(user.BusinessPhone)) newUser.BusinessPhone = user.BusinessPhone;
                    if (user.HomePhone != null && util.stringValid(user.HomePhone)) newUser.HomePhone = user.HomePhone;
                    if (user.PersonalEmail != null && util.stringValid(user.PersonalEmail)) newUser.PersonalEmail = user.PersonalEmail;
                    if (user.BankName != null && util.stringValid(user.BankName)) newUser.BankName = user.BankName;
                    if (user.BankBranchNumber != null && util.stringValid(user.BankBranchNumber)) newUser.BankBranchNumber = user.BankBranchNumber;
                    if (user.BankBranchName != null && util.stringValid(user.BankBranchName)) newUser.BankBranchName = user.BankBranchName;
                    if (user.BankAccountNumber != null && util.stringValid(user.BankAccountNumber)) newUser.BankAccountNumber = user.BankAccountNumber;
                    if (user.BankAccountName != null && util.stringValid(user.BankAccountName)) newUser.BankAccountName = user.BankAccountName;
                    if (user.Street != null && util.stringValid(user.Street)) newUser.Street = user.Street;
                    if (user.FlatNumber != null && util.stringValid(user.FlatNumber)) newUser.FlatNumber = user.FlatNumber;
                    if (user.City != null && util.stringValid(user.City)) newUser.City = user.City;
                    if (user.Province != null && util.stringValid(user.Province)) newUser.Province = user.Province;
                    if (user.PostalCode != null && util.stringValid(user.PostalCode)) newUser.PostalCode = user.PostalCode;
                    if (user.Country != null && util.stringValid(user.Country)) newUser.Country = user.Country;
                    if (user.MartialStatus != null && util.stringValid(user.MartialStatus)) newUser.MartialStatus = user.MartialStatus;
                    if (user.ContactName != null && util.stringValid(user.ContactName)) newUser.ContactName = user.ContactName;
                    if (user.Relationship != null && util.stringValid(user.Relationship)) newUser.Relationship = user.Relationship;
                    if (user.PhoneR != null && util.stringValid(user.PhoneR)) newUser.PhoneR = user.PhoneR;
                    if (user.StreetR != null && util.stringValid(user.StreetR)) newUser.StreetR = user.StreetR;
                    if (user.FlatNumberR != null && util.stringValid(user.FlatNumberR)) newUser.FlatNumberR = user.FlatNumberR;
                    if (user.CityR != null && util.stringValid(user.CityR)) newUser.CityR = user.CityR;
                    if (user.ProvinceR != null && util.stringValid(user.ProvinceR)) newUser.ProvinceR = user.ProvinceR;
                    if (user.PostalCodeR != null && util.stringValid(user.PostalCodeR)) newUser.PostalCodeR = user.PostalCodeR;
                    if (user.CountryR != null && util.stringValid(user.CountryR)) newUser.CountryR = user.CountryR;

                    _db.Users.Add(newUser);

                    _db.SaveChanges();

                    var addUserRoleResult = urService.addUserRoles(newUser.Id.ToString(), user.Roles);
                    if (!addUserRoleResult.Success)
                    {
                        return new Result<Account>(false, addUserRoleResult.Message);
                    }

                    var addDepartmentMemberResult = dms.addDepartmentMembers(newUser.Id.ToString(), user.Departments);
                    if (!addDepartmentMemberResult.Success)
                    {
                        return new Result<Account>(false, addDepartmentMemberResult.Message);
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

        public Result<Account> editProfileService(string idUserEdit, AccountForAddDTO editUser)
        {
            try
            {
                var userId = Guid.Parse(idUserEdit);
                var user = _db.Users.FirstOrDefault(u => u.IsDeleted == false && u.Id == userId);
                if (user == null)
                {
                    return new Result<Account>(false, "User do not exist !");
                }
                //Trace.WriteLine($"--AP: {user.AvatarPath}");

                //start edit fields
                if(editUser.Roles.Length > 0)
                {
                    urService.addUserRoles(idUserEdit, editUser.Roles);
                }
                if(editUser.Departments.Length > 0)
                {
                    dms.addDepartmentMembers(idUserEdit, editUser.Departments);
                }
                if (editUser.Birthday != null)
                {
                    try
                    {
                        user.Birthday = editUser.Birthday;
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
                if (editUser.StartingDateOfficial != null)
                {
                    try
                    {
                        user.StartingDateOfficial = editUser.StartingDateOfficial;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Starting Date Official");
                    }
                }
                if (editUser.LeavingDate != null)
                {
                    try
                    {
                        user.LeavingDate = editUser.LeavingDate;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Leaving Date");
                    }
                }
                if (editUser.StartDateMaternityLeave != null)
                {
                    try
                    {
                        user.StartDateMaternityLeave = editUser.StartDateMaternityLeave;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Wrong format of Start Date Maternity Leave");
                    }
                }

                if (editUser.Email != null && util.stringValid(editUser.Email) && !editUser.Email.Equals(user.Email))
                {
                    if(_db.Users.Where(u => u.Email == editUser.Email).Any())
                    {
                        return new Result<Account>(false, "Duplicate email, try changing to another one !");
                    }
                    if (!util.IsValidEmail(editUser.Email))
                    {
                        return new Result<Account>(false, "Wrong format of email !");
                    }
                    user.Email = editUser.Email;
                    user.Username = editUser.Email;
                }
                if (editUser.EmployeeNumber != null && util.stringValid(editUser.EmployeeNumber) && !editUser.EmployeeNumber.Equals(user.EmployeeNumber))
                {
                    if (_db.Users.FirstOrDefault(u => u.IsDeleted == false && u.EmployeeNumber == editUser.EmployeeNumber) != null)
                    {
                        return new Result<Account>(false, "Duplicate employee number !");
                    }
                    user.EmployeeNumber = editUser.EmployeeNumber;
                }
                if (editUser.AvatarPath != null && util.stringValid(editUser.AvatarPath))
                {
                    user.AvatarPath = editUser.AvatarPath;
                }
                if (editUser.FirstName != null && util.stringValid(editUser.FirstName)) user.FirstName = editUser.FirstName;
                if (editUser.LastName != null && util.stringValid(editUser.LastName)) user.LastName = editUser.LastName;
                if (editUser.Sex != null)
                {
                    try
                    {
                        user.Sex = editUser.Sex;
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        return new Result<Account>(false, "Type of sex is invalid !");
                    }
                }
                if (editUser.JobTitle != null && util.stringValid(editUser.JobTitle)) user.JobTitle = editUser.JobTitle;
                if (editUser.Company != null && util.stringValid(editUser.Company)) user.Company = editUser.Company;
                if (editUser.Unit != null && util.stringValid(editUser.Unit)) user.Unit = editUser.Unit;
                if (editUser.Function != null && util.stringValid(editUser.Function)) user.Function = editUser.Function;
                if (editUser.SectionsOrTeam != null && util.stringValid(editUser.SectionsOrTeam)) user.SectionsOrTeam = editUser.SectionsOrTeam;
                if (editUser.Groups != null && util.stringValid(editUser.Groups)) user.Groups = editUser.Groups;
                if (editUser.OfficeLocation != null && util.stringValid(editUser.OfficeLocation)) user.OfficeLocation = editUser.OfficeLocation;
                if (editUser.LineManager != null && util.stringValid(editUser.LineManager)) user.LineManager = editUser.LineManager;
                if (editUser.BelongToDepartments != null && util.stringValid(editUser.BelongToDepartments)) user.BelongToDepartments = editUser.BelongToDepartments;
                if (editUser.DateOfIdCard != null)
                {
                    try
                    {
                        user.DateOfIdCard = editUser.DateOfIdCard;
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

                if (editUser.StartingDate != null)
                {
                    try
                    {
                        user.StartingDate = editUser.StartingDate;
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
                if (editUser.Rank != null && util.stringValid(editUser.Rank)) user.Rank = editUser.Rank;
                if (editUser.EmployeeType != null && util.stringValid(editUser.EmployeeType)) user.EmployeeType = editUser.EmployeeType;
                if (editUser.Rights != null && util.stringValid(editUser.Rights)) user.Rights = editUser.Rights;
                if (editUser.Nation != null && util.stringValid(editUser.Nation)) user.Nation = editUser.Nation;
                if (editUser.Phone != null && util.stringValid(editUser.Phone)) user.Phone = editUser.Phone;
                if (editUser.IdCardNumber != null && util.stringValid(editUser.IdCardNumber)) user.IdCardNumber = editUser.IdCardNumber;
                if (editUser.PlaceOfIdCard != null && util.stringValid(editUser.PlaceOfIdCard)) user.PlaceOfIdCard = editUser.PlaceOfIdCard;
                if (editUser.HealthInsurance != null && util.stringValid(editUser.HealthInsurance)) user.HealthInsurance = editUser.HealthInsurance;
                if (editUser.Note != null && util.stringValid(editUser.Note)) user.Note = editUser.Note;
                if (editUser.AcademicLevel != null && util.stringValid(editUser.AcademicLevel)) user.AcademicLevel = editUser.AcademicLevel;
                if (editUser.Qualification != null && util.stringValid(editUser.Qualification)) user.Qualification = editUser.Qualification;
                if (editUser.BusinessPhone != null && util.stringValid(editUser.BusinessPhone)) user.BusinessPhone = editUser.BusinessPhone;
                if (editUser.HomePhone != null && util.stringValid(editUser.HomePhone)) user.HomePhone = editUser.HomePhone;
                if (editUser.PersonalEmail != null && util.stringValid(editUser.PersonalEmail)) user.PersonalEmail = editUser.PersonalEmail;
                if (editUser.BankName != null && util.stringValid(editUser.BankName)) user.BankName = editUser.BankName;
                if (editUser.BankBranchNumber != null && util.stringValid(editUser.BankBranchNumber)) user.BankBranchNumber = editUser.BankBranchNumber;
                if (editUser.BankBranchName != null && util.stringValid(editUser.BankBranchName)) user.BankBranchName = editUser.BankBranchName;
                if (editUser.BankAccountNumber != null && util.stringValid(editUser.BankAccountNumber)) user.BankAccountNumber = editUser.BankAccountNumber;
                if (editUser.BankAccountName != null && util.stringValid(editUser.BankAccountName)) user.BankAccountName = editUser.BankAccountName;
                if (editUser.Street != null && util.stringValid(editUser.Street)) user.Street = editUser.Street;
                if (editUser.FlatNumber != null && util.stringValid(editUser.FlatNumber)) user.FlatNumber = editUser.FlatNumber;
                if (editUser.City != null && util.stringValid(editUser.City)) user.City = editUser.City;
                if (editUser.Province != null && util.stringValid(editUser.Province)) user.Province = editUser.Province;
                if (editUser.PostalCode != null && util.stringValid(editUser.PostalCode)) user.PostalCode = editUser.PostalCode;
                if (editUser.Country != null && util.stringValid(editUser.Country)) user.Country = editUser.Country;
                if (editUser.MartialStatus != null && util.stringValid(editUser.MartialStatus)) user.MartialStatus = editUser.MartialStatus;
                if (editUser.ContactName != null && util.stringValid(editUser.ContactName)) user.ContactName = editUser.ContactName;
                if (editUser.Relationship != null && util.stringValid(editUser.Relationship)) user.Relationship = editUser.Relationship;
                if (editUser.PhoneR != null && util.stringValid(editUser.PhoneR)) user.PhoneR = editUser.PhoneR;
                if (editUser.StreetR != null && util.stringValid(editUser.StreetR)) user.StreetR = editUser.StreetR;
                if (editUser.FlatNumberR != null && util.stringValid(editUser.FlatNumberR)) user.FlatNumberR = editUser.FlatNumberR;
                if (editUser.CityR != null && util.stringValid(editUser.CityR)) user.CityR = editUser.CityR;
                if (editUser.ProvinceR != null && util.stringValid(editUser.ProvinceR)) user.ProvinceR = editUser.ProvinceR;
                if (editUser.PostalCodeR != null && util.stringValid(editUser.PostalCodeR)) user.PostalCodeR = editUser.PostalCodeR;
                if (editUser.CountryR != null && util.stringValid(editUser.CountryR)) user.CountryR = editUser.CountryR;
                //user.Created = DateTime.Now;

                _db.SaveChanges();
                return new Result<Account>(true, "Edit profile successfully !", user);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return new Result<Account>(false, "Internal error !");
            }
        }

        public Result<List<AccountRole>> getApprovers()
        {
            var data = _db.UserRoles.Where(u => u.RoleId == 1 || u.RoleId == 2 || u.RoleId == 3).ToList();
            return new Result<List<AccountRole>>(true, "Get all approvers successfully !", data);
        }
    }
}