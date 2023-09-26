using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ZoomaContext _context = new ZoomaContext();

        private readonly IConfiguration _config;//token jwt
        private readonly IMapper _mapper;

        public UsersController(IConfiguration config,IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(short id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(short id, User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'ZoomaContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(short id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(short id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool UserExists(String email)
        {
            return  (_context.Users?.Any(e => e.Email == email)).GetValueOrDefault();
        }


        // ==================================== SIGNUP API ===========================//
 
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("sign-up")]
        public async Task<ActionResult<User>> SignUp(SignUpBody user)
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

            if ( user.Password != user.ConfirmPassword)
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
                RoleId = 3
            };



            _context.Users.Add(signUpUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = signUpUser.Id }, signUpUser);
        }
        // ==================================== UPDATE API ===========================//
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UpdateUserBody updateUserBody)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateUserBody.Email);


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

            return Ok("Update Successfully"); // Return 204 No Content to indicate the request has succeeded
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
        // ==================================== LOGIN API ===========================//
        

        // xác thực bởi token, và sẽ lấy body token ra làm dữ diệu 
        [HttpGet]
        [Route("Launch")]
        [Authorize]

        public async Task<ActionResult<User>> Launch()
        {
            var extractedEmail = GetCurrentEmail();

            if (extractedEmail == null) return NotFound("Token hết hạn");

            var result = GetCurrentUser();

            return Ok(result);
        }

        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var result = _context.Users.FirstOrDefault(x => x.Email == email);
                return result;
            }
            return null;
        }
        //  Lấy ra Email từ token
        private string GetCurrentEmail()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                Console.Write(userClaims.Count());

                foreach (var claim in userClaims)
                {
                    Console.WriteLine(claim.ToString());
                }

                return userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            return null;
        }
        // End



        // tạo ra token dựa trên account
        private string GenerateToken(User user)
        {
            string role = "";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            if (user.RoleId == 1)
            {
                role = "Admin";
            }else if (user.RoleId == 2)
            {
                role = "Staff";
            }

                var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Email),
                new Claim(ClaimTypes.Role,role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);


        }



        [AllowAnonymous]
        [HttpPost("Login")]
        // dòng Task<ActionResult<Account>> hơi dài, nhưng mà ta chỉ cần để ý tới cái trong cùng, tức là Account
        // hàm này sẽ trả về 1 cái tài khoản
        public async Task<ActionResult<LoginResponse>> Login(LoginBody body)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set from Zooma context's User is null.");
            }

            //User là cái bảng User trong db
            //First Or Default Async là hàm tìm cái đầu tiên trong db, nếu có thì trả về User, còn nếu không trả về null
            var userChecking = await _context.Users.FirstOrDefaultAsync(

                // code trong xanh là cây
                // sẽ kiểm tra mỗi row trong cái bảng User
                // nếu thuộc tính email của nó == thuộc tính email trong body + thuộc tính password của nó == thuộc tính password trong body
                // thì sẽ trả về cái row đó => LINQ 
                row =>
                row.Email == body.Email && row.Password == body.Password
                //
                );




            if (userChecking != null)
            {
                // trả về mã 200, và với kết quả thành công
                var loginUser = _mapper.Map<UserDTO>(userChecking);
                String accessToken = GenerateToken(userChecking);

                return Ok(new LoginResponse()
                {
                    AccessToken = accessToken,

                    user = loginUser
                    // tạo ra accessToken dựa trên tài khoản
                }); 

            }
                

            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            // trả về mã lỗi 404
            return NotFound("The User is not existed");
        }

    }
}
