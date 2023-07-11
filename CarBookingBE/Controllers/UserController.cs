using CarBookingTest.Models;
using CarBookingTest.Utils;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.ModelBinding;

namespace CarBookingTest.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        MyDbContext db = new MyDbContext();
        HandlePassword hp = new HandlePassword();
        TokenProps jwt = new TokenProps();
        Guid curUserId;

        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUser([FromBody] Account user)
        {
            if (user == null || user.Username == null || user.Password == null)
            {
                return Json(new { Success = false, Message = "Missing parameter !" });
            }
            else if (user.Username.Trim().Length == 0) {
                return Ok(new { Success = false, Message = "Username is required !" });
            }
            else if (user.Password.Trim().Length == 0)
            {
                return Ok(new { Success = false, Message = "Password is required !" });
            }
            else
            {
                string hash = hp.HashPassword(user.Password);
                var validUser = db.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == hash);
                if(validUser != null)
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
                    curUserId = validUser.Id;

                    return Ok(new { Success = true, Message = "Login successfully !", Token = jwtToken });
                }
                else
                {
                    return Ok(new { Success = false, Message = "Username or password is not correct !" });
                }
            }
        }
        

        [HttpPost]
        [Route("register")]
        [JwtAuthorize]
        public IHttpActionResult registerUser([FromBody] Account user)
        {
            if (user == null)
            {
                return Ok(new { Success = false, Message = "Missing all parameters !" });
            }
            if (user.Username == null || user.Username.Trim().Length == 0)
            {
                return Ok(new { Success = false, Message = "Username is required !" });
            }
            else if(db.Users.FirstOrDefault(u => u.Username == user.Username) != null)
            {
                return Ok(new { Success = false, Message = "This username has existed !" });
            }
            else if (user.Password == null || user.Password.Trim().Length == 0)
            {
                return Ok(new { Success = false, Message = "Password is required !" });
            }
            else if (user.Email == null || user.Email.Trim().Length == 0)
            {
                return Ok(new { Success = false, Message = "Email is required !" });
            }
            else if (user.Sex.ToString() == null || user.Sex.ToString().Trim().Length == 0)
            {
                return Ok(new { Success = false, Message = "Sex is required !" });
            }
            else if (user.EmployeeNumber == null || user.EmployeeNumber.Trim().Length == 0)
            {
                return Ok(new { Success = false, Message = "Employee number is required !" });
            }
            else if (db.Users.FirstOrDefault(u => u.EmployeeNumber == user.EmployeeNumber) != null)
            {
                return Ok(new { Success = false, Message = "This employee number has existed !" });
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
                
                return Ok(new { Success = true, Message = "Register new user successfully !", Data = newUser });
            }
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllUser()
        {
            var users = db.Users.Where(u => u.IsDeleted == false).ToList();
            if (users == null)
            {
                return Ok(new { Success = false, Message = "Get all users fail !", Data = new List<Account>() });
            }
            return Ok(new { Success = true, Message = "Get all users successfully !", Data = users });
        }

        [HttpGet]
        [Route("profile/{id}")]
        public IHttpActionResult getProfile(Guid id)
        {
            var user = db.Users.Find(id);
            if (user == null || user.IsDeleted == true)
            {
                return Ok(new { Success = false, Message = "User does not exist", Data = new List<Account>() });
            }
            var jsonString = JsonSerializer.Serialize(user);
            JObject jsonObject = JObject.Parse(jsonString);
            jsonObject.Remove("Password");
            return Ok(new { Success = true, Message = "Get user profile successfully !", Data = jsonObject });
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult logoutUser([FromBody] TokenProps jwtToken)
        {
            //var token = jwt.UpdateTokenExpiration(jwtToken.tokenForLogout, jwt.secretKey, 0);
            return Ok(new { Success = true, Message = "Logout successfully !", check = jwt.expirationMinutes * (-1) });
        }

        [HttpPost]
        [Route("edit")]
        public IHttpActionResult Post()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                var postedFile = httpRequest.Files[0];
                var filePath = HttpContext.Current.Server.MapPath($"~/Files/Avatar/{curUserId}/{postedFile.FileName}");
                postedFile.SaveAs(filePath);
                docfiles.Add(filePath);
                return Ok(new { Success = true, Message = "Upload success" });
            }
            return Ok(new { Success = false, Message = "Upload fail" });
        }

    }
}