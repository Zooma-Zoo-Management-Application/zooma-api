using AutoMapper;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            // CreateMap<Animal, AnimalDTO>(); Template để mapping dữ liệu giữa Model trong DB và DTO

        }
    }
}
