using AutoMapper;
using PedroTer7.MagicShelf.Api.Service.Dtos;
using PedroTer7.MagicShelf.Api.ViewModels.Out;

namespace PedroTer7.MagicShelf.Api.Config.Mappings
{
    public class ServiceToPresentationMappingProfile : Profile
    {
        public ServiceToPresentationMappingProfile()
        {
            CreateMap<ItemEnumerationDto, ShelfItemListingOutViewModel>();
            CreateMap<CompleteShelfItemOutViewModel, CompleteItemDto>();
            CreateMap<ItemCommentDto, ItemCommentOutViewModel>();
        }
    }
}
