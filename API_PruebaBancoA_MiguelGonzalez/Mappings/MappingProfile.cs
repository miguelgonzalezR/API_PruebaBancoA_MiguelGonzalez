using API_PruebaBancoA_MiguelGonzalez.DTO;
using API_PruebaBancoA_MiguelGonzalez.Models;
using AutoMapper;

namespace API_PruebaBancoA_MiguelGonzalez.Mappings

{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<Tarjetas, TarjetaDTO>()
                .ForMember(dest => dest.SaldoActual, opt => opt.MapFrom(src => src.SaldoActual))
                .ForMember(dest => dest.SaldoDisponible, opt => opt.MapFrom(src => src.SaldoDisponible));
            CreateMap<TarjetaDTO, Tarjetas>();

            CreateMap<Compras, ComprasDTO>();
            CreateMap<ComprasDTO, Compras>();

            CreateMap<Pagos, PagosDTO>();
            CreateMap<PagosDTO, Pagos>();
        }

    }
}
