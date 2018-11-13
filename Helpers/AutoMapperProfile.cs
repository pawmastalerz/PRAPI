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

            CreateMap<Car, CarForUpdateDto>();
            CreateMap<CarForUpdateDto, Car>();

            CreateMap<Car, CarForCreateDto>();
            CreateMap<CarForCreateDto, Car>();
        }
    }
}