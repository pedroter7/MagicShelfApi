using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PedroTer7.MagicShelf.Api.Data.DataContexts;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Data.Entities;
using PedroTer7.MagicShelf.Api.Data.Exceptions;
using PedroTer7.MagicShelf.Api.Data.Repositories.Interfaces;

namespace PedroTer7.MagicShelf.Api.Data.Repositories.Implementations
{
    public class ItemRepository : IItemRepository
    {
        private readonly IMapper _mapper;
        private readonly MagicShelfDbContext _dbContext;

        public ItemRepository(IMapper mapper, MagicShelfDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }


        public async Task<long> AddCommentToItem(long itemId, CommentInDto comment)
        {
            var item = await GetItemThrowsIfNotFound(itemId);
            var commentEntity = _mapper.Map<Comment>(comment);
            commentEntity.ItemId = item.Id;
            commentEntity.Item = item;
            commentEntity.CreatedDate = DateTime.Now;
            if (item.Comments is null)
                item.Comments = new List<Comment>();
            item.Comments.Add(commentEntity);
            _dbContext.Items.Update(item);
            await _dbContext.SaveChangesAsync();
            return commentEntity.Id;
        }

        public Task<int> CountStoredItems()
        {
            return _dbContext.Items.CountAsync();
        }

        public async Task<ICollection<ItemOutDto>> GetAll()
        {
            var items = await _dbContext.Items.Include(i => i.Comments)
                .ToListAsync();
            if (items is null)
                throw new EntityNotFoundException("Item", "all entities");

            return _mapper.Map<ICollection<ItemOutDto>>(items);
        }

        public async Task<ICollection<CommentOutDto>> GetCommentsForItem(long itemId)
        {
            var item = await GetItemThrowsIfNotFound(itemId);
            await LazyLoadCommentsForItem(item);
            if (item.Comments is null || item.Comments.Count == 0)
                throw new EntityNotFoundException("Comment", $"comments for item with id {itemId}");

            return _mapper.Map<ICollection<CommentOutDto>>(item.Comments);
        }

        public async Task<ItemOutDto> GetItem(long id)
        {
            var itemEntity = await GetItemThrowsIfNotFound(id);
            await LazyLoadCommentsForItem(itemEntity);
            return _mapper.Map<ItemOutDto>(itemEntity);
        }

        public async Task<string> GetItemContent(long id)
        {
            var item = await GetItemThrowsIfNotFound(id);
            return item.Content;
        }

        public async Task RemoveItem(long id)
        {
            var itemEntity = await GetItemThrowsIfNotFound(id);
            _dbContext.Items.Remove(itemEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<long> StoreItem(ItemInDto item)
        {
            var itemEntity = _mapper.Map<Item>(item);
            itemEntity.CreatedDate = DateTime.Now;
            _dbContext.Items.Add(itemEntity);
            await _dbContext.SaveChangesAsync();
            return itemEntity.Id;
        }

        private async Task<Item> GetItemThrowsIfNotFound(long id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item is null)
                throw new EntityNotFoundException("Item", $"Id = {id}");
            return item;
        }

        private Task LazyLoadCommentsForItem(Item itemEntity)
        {
            return _dbContext.Entry(itemEntity)
                .Collection(i => i.Comments).LoadAsync();
        }
    }
}
