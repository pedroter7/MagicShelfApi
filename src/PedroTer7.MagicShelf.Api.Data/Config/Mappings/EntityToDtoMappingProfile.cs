using AutoMapper;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Data.Entities;

namespace PedroTer7.MagicShelf.Api.Data.Config.Mappings
{
    public class EntityToDtoMappingProfile : Profile
    {
        public EntityToDtoMappingProfile()
        {
            CreateMap<Item, ItemOutDto>()
                .ForMember(dto => dto.DateStored, mo => mo.MapFrom(e => e.CreatedDate))
                .ForMember(dto => dto.NumberCommentsAbout, mo => mo.MapFrom(e => e.Comments.Count));

            CreateMap<Comment, CommentOutDto>()
                .ForMember(dto => dto.Text, mo => mo.MapFrom(e => e.Details));
        }
    }
}
