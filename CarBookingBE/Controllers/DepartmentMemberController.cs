using CarBookingBE.Services;
using System.Web.Http;

namespace CarBookingBE.Controllers
{
    [RoutePrefix("api/departmentMember")]
    public class DepartmentMemberController : ApiController
    {
        DepartmentMemberService dms = new DepartmentMemberService();
        [HttpGet]
        [Route("all")]
        public IHttpActionResult getAll(int page, int limit)
        {
            return Ok();
        }
    }
}