using AutoMapper;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Helper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Animal, AnimalDTO>().ReverseMap();
        }
    }
}
