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
        UtilMethods util = new UtilMethods();
        string curUserId = "quang";
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
        public Result<Account> registerService(HttpPostedFile file, Account user)
        {
            try
            {
                if(user == null)
                {
                    return new Result<Account>(false, "Missing all parameters !");
                }
                if(user.Username == null || user.Username.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Username is required !");
                }
                else if(_db.Users.FirstOrDefault(u => u.Username == user.Username) != null)
                {
                    return new Result<Account>(false, "This username has existed !");
                }
                else if(user.Password == null || user.Password.Trim().Length == 0)
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
                else if(user.Email == null || user.Email.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Email is required !");
                }
                else if(user.Sex.ToString() == null || user.Sex.ToString().Trim().Length == 0)
                {
                    return new Result<Account>(false, "Sex is required !");
                }
                else if(user.EmployeeNumber == null || user.EmployeeNumber.Trim().Length == 0)
                {
                    return new Result<Account>(false, "Employee number is required !");
                }
                else if(_db.Users.FirstOrDefault(u => u.EmployeeNumber == user.EmployeeNumber) != null)
                {
                    return new Result<Account>(false, "This employee number has existed !");
                }
                else
                {
                    string flag = "";
                    if(file != null && file.ContentType != null && file.FileName.Count() != 0)
                    {
                        var rs = uploadAvatar(file);
                        if(!rs.Success)
                        {
                            return new Result<Account>(false, rs.Message);
                        }
                        flag = rs.Data;
                    }
                    Account newUser = new Account();
                    newUser.Username = user.Username;
                    newUser.Password = hp.HashPassword(user.Password);
                    newUser.Email = user.Email;
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Sex = user.Sex;
                    newUser.EmployeeNumber = user.EmployeeNumber;
                    newUser.IsDeleted = false;
                    newUser.Created = DateTime.Now;
                    if(flag.Length > 0)
                    {
                        newUser.AvatarPath = flag;
                    }
                    else
                    {
                        newUser.AvatarPath = "Files/default-user-profile.png";
                    }
                    _db.Users.Add(newUser);
                    _db.SaveChanges();

                    return new Result<Account>(true, "Register new user successfully !", newUser);
                }
            }
            catch(Exception e)
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
                if(users == null)
                {
                    return new Result<Pagination<Account>>(true, "There's no data !", new Pagination<Account>());
                }
                return new Result<Pagination<Account>>(true, "Get all users successfully !", usersPagination);
            }
            catch(Exception e)
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
                if(user == null || user.IsDeleted == true)
                {
                    return new Result<Account>(false, "User does not exist");
                }
                return new Result<Account>(true, "Get user profile successfully !", user);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<Account>(false, "Internal error !");
            }
        }

        public Result<Account> editProfileService(HttpPostedFile postedFile, string updateUserId, NameValueCollection updateUser)
        {
            try
            {
                var updateId = Guid.Parse(updateUserId);
                var user = _db.Users.Find(updateId);
                if (user == null || user.IsDeleted == true)
                {
                    return new Result<Account>(false, "User do not exist !");
                }

                var userRoles = _db.UserRoles.Where(r => r.IsDeleted == false && r.UserId == updateId).ToList();
                if (!userRoles.Any())
                {
                    return new Result<Account>(false, "User do not have any roles !");
                }

                bool isAdmin = false;
                foreach (var r in userRoles)
                {
                    var rTitle = _db.Roles.Find(r.RoleId);
                    if (rTitle.Title.Equals("ADMIN"))
                    {
                        isAdmin = true;
                    }
                }

                if (postedFile != null)
                {
                    var rs = uploadAvatar(postedFile);
                    if (!rs.Success)
                    {
                        return new Result<Account>(false, rs.Message);
                    }
                    user.AvatarPath = rs.Data;
                }

                if (isAdmin)
                {
                    //Password, IsDeleted
                    if (updateUser["Birthday"] != null) user.Birthday = DateTime.ParseExact(updateUser["Birthday"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (updateUser["StartingDateOfficial"] != null) user.StartingDateOfficial = DateTime.ParseExact(updateUser["StartingDateOfficial"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (updateUser["LeavingDate"] != null) user.LeavingDate = DateTime.ParseExact(updateUser["LeavingDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (updateUser["StartDateMaternityLeave"] != null) user.StartDateMaternityLeave = DateTime.ParseExact(updateUser["StartDateMaternityLeave"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    if (updateUser["Username"] != null) user.Username = updateUser["Username"];
                    if (updateUser["Email"] != null) user.Email = updateUser["Email"];
                    if (updateUser["EmployeeNumber"] != null) user.EmployeeNumber = updateUser["EmployeeNumber"];
                    if (updateUser["AvatarPath"] != null) user.AvatarPath = updateUser["AvatarPath"];
                    if (updateUser["FirstName"] != null) user.FirstName = updateUser["FirstName"];
                    if (updateUser["LastName"] != null) user.LastName = updateUser["LastName"];
                    if (updateUser["Sex"] != null) user.Sex = bool.Parse(updateUser["Sex"]);
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
                if (updateUser["DateOfIdCard"] != null) user.DateOfIdCard = DateTime.ParseExact(updateUser["DateOfIdCard"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (updateUser["StartingDate"] != null) user.StartingDate = DateTime.ParseExact(updateUser["StartingDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

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

        public Result<string> uploadAvatar(HttpPostedFile postedFile)
        {
            try
            {
                string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg" };
                if(postedFile == null || postedFile.FileName.Length == 0)
                {
                    return new Result<string>(false, "Missing file !");
                }
                if(!acceptExtensionImg.Contains(Path.GetExtension(postedFile.FileName)))
                {
                    return new Result<string>(false, "Not support file type ! Please provide image file(.png, .jpg, .jpeg)");
                }
                if(postedFile.ContentLength >(2 * 1024 * 1024))
                {
                    return new Result<string>(false, "The maximum size of file is 20MB !");
                }
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Avatar"), curUserId);
                if(!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"Files/Avatar/{curUserId}/{postedFile.FileName}");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !"); ;
            }
        }
    }
}