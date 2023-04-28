using AutoMapper;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Data.Entities;

namespace PedroTer7.MagicShelf.Api.Data.Config.Mappings
{
    public class DtoToEntityMappingProfile : Profile
    {
        public DtoToEntityMappingProfile()
        {
            CreateMap<CommentInDto, Comment>()
                .ForMember(e => e.Details, mo => mo.MapFrom(dto => dto.Text));

            CreateMap<ItemInDto, Item>()
                .ForMember(e => e.Comments, mo => mo.MapFrom(dto => new List<Comment>()));
        }
    }
}
