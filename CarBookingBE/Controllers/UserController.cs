﻿using CarBookingBE.DTOs;
using CarBookingBE.Services;
using CarBookingBE.Utils;
using CarBookingTest.Models;
using CarBookingTest.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace CarBookingTest.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        UserService userService = new UserService();
        FileService fileService = new FileService();
        UtilMethods util = new UtilMethods();
        RoleConstants roleConstants;

        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUser([FromBody] LoginDTO user)
        {
            return Ok(userService.loginService(user));
        }

        [HttpPost]
        [Route("register")]
        [JwtAuthorize]
        public HttpResponseMessage registerUser()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if (!isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
            }
            var curId = isAuthorized.Data;
            var httpRequest = HttpContext.Current.Request;
            Account user = new Account();
            user.Email = httpRequest.Form["Email"];
            user.Password = httpRequest.Form["Password"];
            user.FirstName = httpRequest.Form["FirstName"];
            user.LastName = httpRequest.Form["LastName"];
            user.Sex = bool.Parse(httpRequest.Form["Sex"]);
            user.EmployeeNumber = httpRequest.Form["EmployeeNumber"];
            string roleId = httpRequest.Form["Role"];
            if (httpRequest.Files.Count == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, userService.registerService(curId, httpRequest.Files[0], user, roleId));
            }
            return Request.CreateResponse(HttpStatusCode.OK, userService.registerService(curId, null, user, roleId));
        }

        [HttpGet]
        [Route("all")]
        [JwtAuthorize]
        public IHttpActionResult getAllUser(int page, int limit)
        {
            return Ok(userService.getUsersService(page, limit));
        }

        [HttpGet]
        [Route("profile/{id}")]
        [JwtAuthorize]
        public HttpResponseMessage getProfile(string id)
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            var curId = isAuthorized.Data != null ? isAuthorized.Data : new Guid();
            // users can view profile of themselves
            if (curId == Guid.Parse(id) || isAuthorized.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, userService.getProfileService(id));
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
        }

        /*[HttpPost]
        [Route("logout")]
        [JwtAuthorize]
        public IHttpActionResult logoutUser([FromBody] TokenProps jwtToken)
        {
            //var token = jwt.UpdateTokenExpiration(jwtToken.tokenForLogout, jwt.secretKey, 0);
            return Ok(new { Success = true, Message = "Logout successfully !" });
        }*/

        [HttpPut]
        [Route("edit/{idEdit}")]
        [JwtAuthorize]
        public HttpResponseMessage editProfile(string idEdit)
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            var curId = isAuthorized.Data != null ? isAuthorized.Data : new Guid();

            // users can edit profile of themselves or admin
            if (curId == Guid.Parse(idEdit) || isAuthorized.Success)
            {
                var httpRequest = HttpContext.Current.Request;
                var formData = httpRequest.Form;
                if (httpRequest.Files.Count == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, userService.editProfileService(curId, httpRequest.Files[0], idEdit, formData));
                }
                return Request.CreateResponse(HttpStatusCode.OK, userService.editProfileService(curId, null, idEdit, formData));
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Unauthorized request !" });
        }

        [HttpPost]
        [Route("testUpload")]
        public IHttpActionResult testUpload()
        {
            var isAuthorized = util.isAuthorized(new RoleConstants(true, false, false, false, false));
            if(!isAuthorized.Success)
            {
                return Unauthorized();
            }
            var curId = isAuthorized.Data;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count == 1)
            {
                return Ok(fileService.uploadAvatar(curId, httpRequest.Files[0]));
            }
            return BadRequest();
        }
    }
}