using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;

namespace CarBookingBE.Services
{
    public class UserService
    {
        MyDbContext db = new MyDbContext();
        HandlePassword hp = new HandlePassword();
        TokenProps jwt = new TokenProps();
        string curUserId = "quang";

        public JObject parseToJson(object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            return JObject.Parse(jsonString); 
        }
        public Result<string> loginService(Account user)
        {
            if (user == null || user.Username == null || user.Password == null)
            {
                return new Result<string>(false, "Missing required parameter !");
            }
            else if (user.Username.Trim().Length == 0)
            {
                return new Result<string>(false, "Username is required !");
            }
            else if (user.Password.Trim().Length == 0)
            {
                return new Result<string>(false, "Password is required !");
            }
            else
            {
                string hash = hp.HashPassword(user.Password);
                var validUser = db.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == hash);
                if (validUser != null)
                {
                    string secretKey = jwt.secretKey;
                    string issuer = jwt.issuer;
                    string audience = jwt.audience;
                    int expirationMinutes = jwt.expirationMinutes;
                    Claim[] customClaims = new Claim[]
                    {
                    new Claim("Username", user.Username),
                    new Claim("Password", user.Password)
                    };
                    string jwtToken = jwt.GenerateJwtToken(secretKey, issuer, audience, expirationMinutes, customClaims);
                    //bool check = jwt.VerifyJwtToken(jwtToken, secretKey);

                    return new Result<string>(true, "Login successfully !", jwtToken);
                }
                else
                {
                    return new Result<string>(false, "Username or password is not correct !");
                }
            }
        }
        public Result<Account> registerService(HttpPostedFile file, Account user)
        {
            if (user == null)
            {
                return new Result<Account>(false, "Missing all parameters !");
            }
            if (user.Username == null || user.Username.Trim().Length == 0)
            {
                return new Result<Account>(false, "Username is required !");
            }
            else if (db.Users.FirstOrDefault(u => u.Username == user.Username) != null)
            {
                return new Result<Account>(false, "This username has existed !");
            }
            else if (user.Password == null || user.Password.Trim().Length == 0)
            {
                return new Result<Account>(false, "Password is required !");
            }
            else if (user.Email == null || user.Email.Trim().Length == 0)
            {
                return new Result<Account>(false, "Email is required !");
            }
            else if (user.Sex.ToString() == null || user.Sex.ToString().Trim().Length == 0)
            {
                return new Result<Account>(false, "Sex is required !");
            }
            else if (user.EmployeeNumber == null || user.EmployeeNumber.Trim().Length == 0)
            {
                return new Result<Account>(false, "Employee number is required !");
            }
            else if (db.Users.FirstOrDefault(u => u.EmployeeNumber == user.EmployeeNumber) != null)
            {
                return new Result<Account>(false, "This employee number has existed !");
            }
            else
            {
                string flag = "";
                if (file != null)
                {
                    var rs = uploadAvatar(file);
                    if (!rs.Success)
                    {
                        return new Result<Account>(false, rs.Message);
                    }
                    flag = rs.Data;
                }
                Account newUser = new Account();
                newUser.Username = user.Username;
                newUser.Password = hp.HashPassword(user.Password);
                newUser.Email = user.Email;
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
                db.Users.Add(newUser);
                db.SaveChanges();

                return new Result<Account>(true, "Register new user successfully !", newUser);
            }
        }
        public Result<List<Account>> getUsersService()
        {
            var users = db.Users.Where(u => u.IsDeleted == false).ToList();
            if (users == null)
            {
                return new Result<List<Account>>(false, "Get all users fail !");
            }
            return new Result<List<Account>>(false, "Get all users successfully !", users);
        }

        public Account getAcc(string id)
        {
            return db.Users.Find(Guid.Parse(id));
        }

        public Result<Account> getProfileService(string id)
        {
            try
            {
                var user = db.Users.Find(Guid.Parse(id));
                if (user == null || user.IsDeleted == true)
                {
                    return new Result<Account>(false, "User does not exist");
                }
                return new Result<Account>(true, "Get user profile successfully !", user);
            }
            catch (Exception ex)
            {
                return new Result<Account>(false, ex.Message);
            }
        }

        public Result<List<AccountRole>> editProfileService(HttpPostedFile postedFile, string updateUserId, Account updateUser)
        {
            var user = db.Users.Find(Guid.Parse(updateUserId));
            if (user == null || user.IsDeleted == true)
            {
                return new Result<List<AccountRole>>(false, "User do not exist !");
            }

            var userRoles = db.UserRoles.Where(r => r.IsDeleted == false && r.UserId.ToString().ToLower() == updateUserId.ToLower()).ToList();
            if (!userRoles.Any())
            {
                return new Result<List<AccountRole>>(false, "User do not have any roles !");
            }
            bool isAdmin = false;
            foreach (var r in userRoles)
            {
                var rTitle = db.Roles.Find(r.RoleId);
                if (rTitle.Title.Equals("ADMIN"))
                {
                    isAdmin = true;
                }
            }

            if (isAdmin)
            {
                foreach (PropertyInfo prop in updateUser.GetType().GetProperties())
                {
                    Trace.WriteLine($"{prop.Name}: {prop.GetValue(updateUser, null)}");
                }
            }
            else
            {
                foreach (PropertyInfo prop in updateUser.GetType().GetProperties())
                {
                    Trace.WriteLine($"{prop.Name}: {prop.GetValue(updateUser, null)}");
                }
            }
            return new Result<List<AccountRole>>(true, "Get user profile successfully ! " + isAdmin, userRoles);
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
                    return new Result<string>(false, "Not support file type !");
                }
                if (postedFile.ContentLength > (2 * 1024 * 1024))
                {
                    return new Result<string>(false, "The maximum size of file is 20MB !");
                }
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Avatar"), curUserId);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"Files/Avatar/{curUserId}/{postedFile.FileName}");
            }
            catch (Exception ex)
            {
                return new Result<string>(false, ex.Message); ;
            }
        }
    }
}