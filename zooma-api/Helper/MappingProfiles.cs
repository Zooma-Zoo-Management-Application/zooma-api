using AutoMapper;
using zooma_api.Controllers;
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
            CreateMap<Animal, AnimalDTO>().ReverseMap();
            CreateMap<Species, SpeciesDTO>().ReverseMap();
            CreateMap<Cage, CagesDTO>().ReverseMap();
            CreateMap<TrainingPlan, TrainingPlanDTO>().ReverseMap();
            CreateMap<CreateSpecies, Species>().ReverseMap();
            CreateMap<Diet, DietDTO>().ReverseMap();
            CreateMap<News, NewsDTO>().ReverseMap();
            CreateMap<News, NewsBody>().ReverseMap();
            CreateMap<Ticket, CartItemDTO>().ReverseMap();
            CreateMap<User, NewsDTO>().ReverseMap();
            CreateMap<Transaction, TransactionDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<Ticket,TicketDTO >().ReverseMap();
            CreateMap<Area, AreaDTO>().ReverseMap();
            CreateMap<Animal, AnimalUpdate>().ReverseMap();
        }
    }
}
