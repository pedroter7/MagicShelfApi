using AutoMapper;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Service.Dtos;

namespace PedroTer7.MagicShelf.Api.Service.Config.Mappings
{
    public class ServiceToDataMappingProfile : Profile
    {
        public ServiceToDataMappingProfile()
        {
            CreateMap<ItemToInsertDto, ItemInDto>();
            CreateMap<ItemCommentToInsertDto, CommentInDto>();
        }
    }
}
