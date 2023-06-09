using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList=new List<VillaDto>()
        {
            new VillaDto{Id=1, Nombre="vista a la picina", Ocupantes=3, Area=50},
            new VillaDto{Id=2, Nombre="vista a la playa", Ocupantes=4, Area=80}
        };
    }
}
