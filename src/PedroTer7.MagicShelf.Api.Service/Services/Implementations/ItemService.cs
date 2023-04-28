using AutoMapper;
using Microsoft.Extensions.Options;
using PedroTer7.MagicShelf.Api.Data.Dtos;
using PedroTer7.MagicShelf.Api.Data.Exceptions;
using PedroTer7.MagicShelf.Api.Data.Repositories.Interfaces;
using PedroTer7.MagicShelf.Api.Service.Config.Options;
using PedroTer7.MagicShelf.Api.Service.Dtos;
using PedroTer7.MagicShelf.Api.Service.Exceptions;
using PedroTer7.MagicShelf.Api.Service.Services.Interfaces;

namespace PedroTer7.MagicShelf.Api.Service.Services.Implementations
{
    public class ItemService : IItemsService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        private readonly ShelfOptions _shelfOptions;

        public ItemService(IItemRepository itemRepository, IMapper mapper, IOptions<ShelfOptions> options)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
            _shelfOptions = options.Value;
        }

        public async Task<IEnumerable<ItemCommentDto>> GetCommentsForItem(long itemId)
        {
            try
            {
                await _itemRepository.GetItem(itemId);
            }
            catch (EntityNotFoundException)
            {
                throw new NoItemOnShelfSatisfiesConditionException($"item with ID={itemId}");
            }
            try
            {
                var comments = await _itemRepository.GetCommentsForItem(itemId);
                return _mapper.Map<IEnumerable<ItemCommentDto>>(comments);
            }
            catch (EntityNotFoundException)
            {
                throw new ItemHasNoCommentsException(itemId);
            }
        }

        public async Task<long> InsertCommentForItem(long itemId, ItemCommentToInsertDto comment)
        {
            try
            {
                var id = await _itemRepository.AddCommentToItem(itemId, _mapper.Map<CommentInDto>(comment));
                return id;
            }
            catch (EntityNotFoundException)
            {
                throw new NoItemOnShelfSatisfiesConditionException($"item with ID={itemId}");
            }
        }

        public async Task<long> InsertItem(ItemToInsertDto item)
        {
            if (!await ThereIsRoomForNewItems())
                throw new ShelfFullException();

            return await _itemRepository.StoreItem(_mapper.Map<ItemInDto>(item));
        }

        public async Task<IEnumerable<ItemEnumerationDto>> ListAllItems()
        {
            try
            {
                var items = await _itemRepository.GetAll();
                return _mapper.Map<IEnumerable<ItemEnumerationDto>>(items);
            }
            catch (EntityNotFoundException)
            {
                throw new EmptyShelfException();
            }
        }

        public async Task<IEnumerable<ItemEnumerationDto>> ListItemsThatContainKeyword(string keyword)
        {
            try
            {
                var items = await _itemRepository.GetAll();
                var itemsFiltered = items.Where(i => i.Name.Contains(keyword) || i.Description.Contains(keyword));
                if (!itemsFiltered.Any())
                    throw new NoItemOnShelfSatisfiesConditionException($"Keyword {keyword} in name or description");

                return _mapper.Map<IEnumerable<ItemEnumerationDto>>(itemsFiltered);
            }
            catch (EntityNotFoundException)
            {
                throw new EmptyShelfException();
            }
        }

        public async Task<CompleteItemDto> PopItem(long id)
        {
            try
            {
                var item = await _itemRepository.GetItem(id);
                var content = await _itemRepository.GetItemContent(id);
                var completeItem = new CompleteItemDto
                {
                    Content = content,
                    DateStored = item.DateStored,
                    Id = item.Id,
                    Name = item.Name,
                    NumberCommentsAbout = item.NumberCommentsAbout,
                    Description = item.Description,
                };
                await _itemRepository.RemoveItem(id);
                return completeItem;
            }
            catch (EntityNotFoundException)
            {
                throw new NoItemOnShelfSatisfiesConditionException($"item with ID={id}");
            }
        }

        public async Task<bool> ThereIsRoomForNewItems()
        {
            var itemsCount = await _itemRepository.CountStoredItems();
            return itemsCount < _shelfOptions.ShelfSize;
        }
    }
}
