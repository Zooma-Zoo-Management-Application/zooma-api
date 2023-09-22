using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase // TEST AUTHORIZE STAFF
    {
        ///// Staff
        [HttpGet]
        [Route("staffs")]
        [Authorize(Roles = "Staff")]
        public IActionResult StaffEndpoint()
        {
            // var role = currentUser.RoleId == 2 ? "Staff" : null;
            return Ok($"Hi you are a staff ");
        }
    }
}
