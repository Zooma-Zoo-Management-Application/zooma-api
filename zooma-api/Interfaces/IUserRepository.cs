using Microsoft.IdentityModel.Tokens;
using zooma_api.Models;
using zooma_api.Repositories;

namespace zooma_api.Interfaces
{
    public interface IUserRepository
    {
        Task<AllUsersQuantity> GetUsersQuantityAsync();
    
    }

   
}
