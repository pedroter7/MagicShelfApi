using PedroTer7.MagicShelf.Api.Service.Dtos;

namespace PedroTer7.MagicShelf.Api.Service.Services.Interfaces
{
    public interface IItemsService
    {
        Task<bool> ThereIsRoomForNewItems();
        Task<IEnumerable<ItemEnumerationDto>> ListAllItems();
        Task<IEnumerable<ItemEnumerationDto>> ListItemsThatContainKeyword(string keyword);
        Task<CompleteItemDto> PopItem(long id);
        Task<long> InsertItem(ItemToInsertDto item);
        Task<IEnumerable<ItemCommentDto>> GetCommentsForItem(long itemId);
        Task<long> InsertCommentForItem(long itemId, ItemCommentToInsertDto comment);
    }
}
