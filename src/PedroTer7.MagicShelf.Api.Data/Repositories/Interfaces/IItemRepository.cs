using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Data.Entities;

namespace PedroTer7.MagicShelf.Api.Data.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Task<ItemOutDto> GetItem(long id);
        Task<ICollection<ItemOutDto>> GetAll();
        Task<long> StoreItem(ItemInDto item);
        Task RemoveItem(long id);
        Task<string> GetItemContent(long id);
        Task<ICollection<CommentOutDto>> GetCommentsForItem(long itemId);
        Task<long> AddCommentToItem(long itemId, CommentInDto comment);
        Task<int> CountStoredItems();
    }
}
