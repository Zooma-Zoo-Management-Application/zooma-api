using System.Security.Principal;
using zooma_api.Models;

namespace zooma_api.DTO
{
    public class LoginResponse
    {
        public UserDTO user { get; set; }
        public string AccessToken { get; set; }
    }
}
