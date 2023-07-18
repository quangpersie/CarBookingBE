using CarBookingBE.Services;
using CarBookingTest.Models;
using System.Web.Http;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/userRole")]
    public class UserRoleController : ApiController
    {
        UserRoleService userRoleService = new UserRoleService();

        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAllUserRole(int page, int limit)
        {
            return Ok(userRoleService.getAllUserRole(page, limit));
        }

        [HttpGet]
        [Route("find")]
        public IHttpActionResult getUserRoleById(AccountRole accRole)
        {
            return Ok(userRoleService.getUserRoleById(accRole));
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult addUserRole([FromBody] AccountRole accRole)
        {
            return Ok(userRoleService.addUserRole(accRole));
        }

        [HttpPut]
        [Route("edit")]
        public IHttpActionResult editUserRole([FromBody] AccountRole accRole)
        {
            return Ok(userRoleService.editUserRole(accRole));
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult deleteUserRole([FromBody] AccountRole accRole)
        {
            return Ok(userRoleService.deleteUserRole(accRole));
        }
    }
}