using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase // TEST AUTHORIZE ADMIN
    {
        private readonly zoomadbContext _context = new zoomadbContext();

        //For admin Only
        [HttpGet]
        [Route("admins")]
        [Authorize]
        public IActionResult AdminEndpoint()
        {
            var currentUser = GetCurrentUser(); 
           // var role = currentUser.RoleId == 1 ? "admin" : null;
            return Ok($"Hi you are an admin ");
        }
        private User  GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var result =  _context.Users.FirstOrDefault(x => x.Email == email);
                return result;
            }
            return null;
        }

        
    }
}
