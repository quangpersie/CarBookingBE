using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace CarBookingTest.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        UserService userService = new UserService();

        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUser([FromBody] LoginDTO user)
        {
            return Ok(userService.loginService(user));
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult registerUser()
        {
            var httpRequest = HttpContext.Current.Request;
            Account user = new Account();
            user.Username = httpRequest.Form["Username"];
            user.Password = httpRequest.Form["Password"];
            user.FirstName = httpRequest.Form["FirstName"];
            user.LastName = httpRequest.Form["LastName"];
            user.Email = httpRequest.Form["Email"];
            user.Sex = bool.Parse(httpRequest.Form["Sex"]);
            user.EmployeeNumber = httpRequest.Form["EmployeeNumber"];
            if (httpRequest.Files.Count == 1)
            {
                return Ok(userService.registerService(httpRequest.Files[0], user));
            }
            return Ok(userService.registerService(null, user));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllUser(int page, int limit)
        {
            return Ok(userService.getUsersService(page, limit));
        }

        [HttpGet]
        [Route("profile/{id}")]
        public IHttpActionResult getProfile(string id)
        {
            return Ok(userService.getProfileService(id));
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult logoutUser([FromBody] TokenProps jwtToken)
        {
            //var token = jwt.UpdateTokenExpiration(jwtToken.tokenForLogout, jwt.secretKey, 0);
            return Ok(new { Success = true, Message = "Logout successfully !" });
        }

        [HttpPut]
        [Route("edit/{id}")]
        public IHttpActionResult editProfile(string id)
        {
            var httpRequest = HttpContext.Current.Request;
            var formData = httpRequest.Form;
            if (httpRequest.Files.Count == 1)
            {
                return Ok(userService.editProfileService(httpRequest.Files[0], id, formData));
            }
            return Ok(userService.editProfileService(null, id, formData));
        }

        [HttpPost]
        [Route("testUpload")]
        public IHttpActionResult testUpload()
        {
            var httpRequest = HttpContext.Current.Request;
            if(httpRequest.Files.Count == 1)
            {
                return Ok(userService.uploadAvatar(httpRequest.Files[0]));
            }
            return BadRequest();
        }
    }
}