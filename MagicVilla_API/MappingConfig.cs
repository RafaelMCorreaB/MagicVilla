using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>();//mapeo en un sentido (fuente, destino)
            CreateMap<VillaDto, Villa>();//mapeo en sentido inverso

            CreateMap<Villa, VillaCreateDto>().ReverseMap();//mapeo en ambos sentidos
            CreateMap<Villa, VillaUpdateDto>().ReverseMap();//mapeo en ambos sentidos



        }
    }
}
