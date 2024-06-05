using AutoMapper;
using FinalUygulama.API.DTOs;
using FinalUygulama.API.Models;

namespace FinalUygulama.API.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<Araba, ArabaDto>().ReverseMap();
            CreateMap<Kiralama, KiralamaDto>().ReverseMap();
        }
    }
}
