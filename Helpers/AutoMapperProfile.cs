using System.Collections.Generic;
using AutoMapper;
using PRAPI.Dtos;
using PRAPI.Models;

namespace PRAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, UserDataDto>();

            CreateMap<Car, CarDetailsDto>();

            CreateMap<OrderParams, Order>();
            CreateMap<Order, OrderDetailDto>();
        }
    }
}