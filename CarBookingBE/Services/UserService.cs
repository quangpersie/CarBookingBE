using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace CarBookingBE.Services
{
    public class UserService
    {
        MyDbContext db = new MyDbContext();
        HandlePassword hp = new HandlePassword();
        TokenProps jwt = new TokenProps();
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
        public Result<Account> registerService(Account user)
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
                Account newUser = new Account();
                newUser.Username = user.Username;
                newUser.Password = hp.HashPassword(user.Password);
                newUser.Email = user.Email;
                newUser.Sex = user.Sex;
                newUser.EmployeeNumber = user.EmployeeNumber;
                newUser.IsDeleted = false;
                newUser.Created = DateTime.Now;

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

        public Result<JObject> getProfileService(string id)
        {
            try
            {
                var user = db.Users.Find(Guid.Parse(id));
                if (user == null || user.IsDeleted == true)
                {
                    return new Result<JObject>(false, "User does not exist");
                }
                var jsonString = JsonSerializer.Serialize(user);
                JObject jsonObject = JObject.Parse(jsonString);
                jsonObject.Remove("Password");
                return new Result<JObject>(true, "Get user profile successfully !", jsonObject);
            }
            catch (Exception ex)
            {
                return new Result<JObject>(false, ex.Message);
            }
        }

        public Result<JObject> editProfileService(HttpRequest httpRequest, string updateUserId)
        {
            var updateUser = db.Users.Find(Guid.Parse(updateUserId));
            if (updateUser == null || updateUser.IsDeleted == true)
            {
                return new Result<JObject>(false, "User do not exist !");
            }

            var roles = db.UserRoles.Where(r => r.IsDeleted == false && r.UserId == Guid.Parse(updateUserId));
            if(!roles.Any())
            {
                return new Result<JObject>(false, "User do not have any roles !");
            }
            foreach (var r in roles)
            {
                var rTitle = db.Roles.Find(r.RoleId).Title;
                /*if (rTitle.Equals("ADMIN") {

                }
                else if (rTitle.Equals("EMPLOYEE"))
                {
                    foreach (PropertyInfo prop in updateUser.GetType().GetProperties())
                    {
                        Trace.WriteLine($"{prop.Name}: {prop.GetValue(updateUser, null)}");
                    }
                }*/
            }
            return new Result<JObject>(true, "Get user profile successfully !");
        }

        public bool uploadAvatar(HttpRequest httpRequest)
        {
            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    var postedFile = httpRequest.Files[0];
                    string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Avatar"), "quang");
                    if (!Directory.Exists(pathToSave))
                    {
                        Directory.CreateDirectory(pathToSave);
                    }
                    postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                    //return new Result<string>(true, "Upload success", pathToSave);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
            //return new Result<string>(false, "Upload fail");
        }
    }
}