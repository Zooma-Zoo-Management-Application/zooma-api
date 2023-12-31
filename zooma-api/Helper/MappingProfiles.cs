﻿using AutoMapper;
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
            CreateMap<Diet, DietDTO>().ReverseMap();
            CreateMap<News, NewsDTO>().ReverseMap();
            CreateMap<Ticket, CartItemDTO>().ReverseMap();
            CreateMap<Transaction, TransactionDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<Ticket,TicketDTO >().ReverseMap();
            CreateMap<Area, AreaDTO>().ReverseMap();
            CreateMap<Animal, AnimalWithCage>().ReverseMap();
            CreateMap<AnimalUser, AnimalUserDTO>().ReverseMap();
            CreateMap<Skill, SkillDTO>().ReverseMap();
            CreateMap<TrainerExp, TrainerExpDTO>().ReverseMap();
            CreateMap<Models.Type, TypeDTO>().ReverseMap();
            CreateMap<Food, FoodDTO>().ReverseMap();
            CreateMap<FoodSpecy, FoodSpecyDTO>().ReverseMap();
            CreateMap<DietDetail, DietDetailDTO>().ReverseMap();
            CreateMap<Cage, CageOnly>().ReverseMap();
            CreateMap<Diet, DietOnly>().ReverseMap();
            CreateMap<TrainerExp, TrainerExpWIthSkillName>().ReverseMap();
        }
    }
}
