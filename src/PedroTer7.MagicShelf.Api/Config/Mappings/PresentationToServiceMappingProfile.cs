using AutoMapper;
using PedroTer7.MagicShelf.Api.Service.Dtos;
using PedroTer7.MagicShelf.Api.ViewModels.In;

namespace PedroTer7.MagicShelf.Api.Config.Mappings
{
    public class PresentationToServiceMappingProfile : Profile
    {
        public PresentationToServiceMappingProfile()
        {
            CreateMap<ItemToStoreInViewModel, ItemToInsertDto>();
            CreateMap<CommentToAddToItemInViewModel, ItemCommentToInsertDto>();
        }
    }
}
