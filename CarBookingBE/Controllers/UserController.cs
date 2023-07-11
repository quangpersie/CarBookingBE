using CarBookingBE.Services;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace CarBookingTest.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        MyDbContext db = new MyDbContext();
        UserService userService = new UserService();

        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUser([FromBody] Account user)
        {
            return Ok(userService.loginService(user));
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult registerUser([FromBody] Account user)
        {
            return Ok(userService.registerService(user));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllUser()
        {
            return Ok(userService.getUsersService());
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

        [HttpPost]
        //[JwtAuthorize]
        [Route("edit/{id}")]
        public IHttpActionResult editProfile(string id)
        {
            var httpRequest = HttpContext.Current.Request;
            return Ok(userService.editProfileService(httpRequest, id));
        }

    }
}