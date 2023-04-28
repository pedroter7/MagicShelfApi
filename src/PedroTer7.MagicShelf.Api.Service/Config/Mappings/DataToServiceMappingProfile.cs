using AutoMapper;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Service.Dtos;

namespace PedroTer7.MagicShelf.Api.Service.Config.Mappings
{
    public class DataToServiceMappingProfile : Profile
    {
        public DataToServiceMappingProfile()
        {
            CreateMap<ItemOutDto, ItemEnumerationDto>();
            CreateMap<CommentOutDto, ItemCommentDto>()
                .ForMember(sDto => sDto.PostedDate, mo => mo.MapFrom(dDto => dDto.CreatedTime));
        }
    }
}
