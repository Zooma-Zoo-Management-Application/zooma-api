using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using zooma_api.DTO;
using zooma_api.Models;
using zooma_api.Repositories;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ZoomaContext _context;
        private readonly IMapper _mapper;

        private IOrderRepository repository = new OrderRepository();


        public AccountsController(IConfiguration configuration, ZoomaContext context, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;

        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers() // CHỈ GET USER BÌNH THƯỜNG STATUS LÀ TRUE
        {
            if (_context.Users == null)
            {
                return NotFound();

            }

            var list = _mapper.Map<ICollection<UserDTO>>(await _context.Users.Where(u => u.Status==true).ToListAsync());

            return Ok(list);
        }

        [HttpGet("banned-users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetBannedUsers() // CHỈ GET USER BÌNH THƯỜNG STATUS LÀ TRUE
        {
            if (_context.Users == null)
            {
                return NotFound();

            }

            var list = _mapper.Map<ICollection<UserDTO>>(await _context.Users.Where(u => u.Status == false).ToListAsync());

            return Ok(list);
        }

        [HttpGet]
        [Route("zoo-trainers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetZooTrainers()
        {
            if (_context.Users == null)
            {
                return NotFound();

            }

            var list =  _mapper.Map<ICollection<UserDTO>>(await _context.Users.Where(u => u.RoleId == 2 ).ToListAsync());

            return Ok(list);
        }

        [HttpGet]
        [Route("staffs")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetStaffs()
        {
            if (_context.Users == null)
            {
                return NotFound();

            }

            var list = _mapper.Map<ICollection<UserDTO>>(await _context.Users.Where(u => u.RoleId == 1).ToListAsync());

            return Ok(list);
        }

        //========================= API BAN VÀ UNBAN USER VỚI BODY EMAIL HOẶC USERID ==========================//

        // DELETE: api/Users/5
        [HttpDelete("ban-user/{id}")]
        public async Task<IActionResult> DeleteUser(short id)
        {

            if (_context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync( //qwdqwdqwdqwdwqd
                row =>
                row.Id == id
                //
                );

            if (user == null)
            {
                return NotFound();
            }

            user.Status = false;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (UserExists(user.Email))  // Assuming email is unique
                {
                    throw;
                }
                else
                {
                    return NotFound("Email không tồn tại");

                }
            }

            return Ok(new { banUser = _mapper.Map<UserDTO>(user), message = "Ban successfully" });
        }

        [HttpPut("unban-user/{id}")]
        public async Task<IActionResult> ReviseUser(short id)
        {

            if (_context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(
                row =>
                row.Id == id
                //
                );

            if (user == null)
            {
                return NotFound();
            }

            user.Status = true;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (UserExists(user.Email))  // Assuming email is unique
                {
                    throw;
                }
                else
                {
                    return NotFound("Email is not existed");

                }
            }

            return Ok(new { banUser = _mapper.Map<UserDTO>(user), message = "Hồi sinh successfully" });
        }


        private bool UserExists(short id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool UserExists(String email)
        {
            return (_context.Users?.Any(e => e.Email == email)).GetValueOrDefault();
        }

        // ==================================== SIGNUP API ===========================//

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create-account")]
        public async Task<ActionResult<UserDTO>> CreateTrainerAndStaff(CreateAccountBody user)
        {
            //                var loginUser = _mapper.Map<UserDTO>(userChecking);


            if (_context.Users == null)
            {
                return Problem("Entity set 'ZoomaContext.Users'  is null.");
            }

            if (UserExists(user.Email))
            {
                return BadRequest("Email already exists");

            }

            if (user.Password != user.ConfirmPassword)
            {
                return BadRequest("Please type the correct confirm password");

            }


            User signUpUser = new User
            {
                // Assuming you have similar fields in your User entity
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                AvatarUrl = user.AvatarUrl,
                Status = true,
                RoleId = user.RoleId // role ID = 1 or role ID = 2
            };
            _context.Users.Add(signUpUser);

            await _context.SaveChangesAsync();

            return Ok(new { user=_mapper.Map<UserDTO>(signUpUser), message = "Account created succesfully" });

        }

        // ==================================== UPDATE API ===========================//
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(UpdateUserBody updateUserBody)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == updateUserBody.id);


            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Update fields from UpdateUserBody
            existingUser.UserName = updateUserBody.UserName ?? existingUser.UserName;
            existingUser.Email = updateUserBody.Email ?? existingUser.Email;
            existingUser.FullName = updateUserBody.FullName ?? existingUser.FullName;
            existingUser.PhoneNumber = updateUserBody.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Gender = updateUserBody.Gender ?? existingUser.Gender;
            existingUser.DateOfBirth = updateUserBody.DateOfBirth ?? existingUser.DateOfBirth;
            existingUser.AvatarUrl = updateUserBody.AvatarUrl ?? existingUser.AvatarUrl;

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(existingUser.Email))  // Assuming email is unique
                {
                    return NotFound("Email đã tồn tại");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { user = _mapper.Map<UserDTO>(existingUser), msg="Update Successfully" }); // Return 204 No Content to indicate the request has succeeded
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordBody updatePassword)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatePassword.Id);


            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            if (existingUser.Password != updatePassword.currentPassword)
            {
                return BadRequest("Your old password is wrong!!");
            }
            else
            {
                // Update password from UpdatePasswordBody
                existingUser.Password = updatePassword.NewPassword;
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(existingUser.Email))  // Assuming email is unique
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update Password Successfully"); // Return 204 No Content to indicate the request has succeeded
        }
    }

    public  class CreateAccountBody
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AvatarUrl { get; set; }
        public byte RoleId { get; set; }
    }
}
