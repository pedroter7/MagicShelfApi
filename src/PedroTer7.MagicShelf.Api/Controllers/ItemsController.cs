using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PedroTer7.MagicShelf.Api.Service.Dtos;
using PedroTer7.MagicShelf.Api.Service.Exceptions;
using PedroTer7.MagicShelf.Api.Service.Services.Interfaces;
using PedroTer7.MagicShelf.Api.ViewModels.In;
using PedroTer7.MagicShelf.Api.ViewModels.Out;
using System.ComponentModel.DataAnnotations;

namespace PedroTer7.MagicShelf.Api.Controllers
{
    [ApiController]
    [Route("api/v1/items")]
    public class ItemsController : Controller
    {
        private readonly IItemsService _itemsService;
        private readonly IMapper _mapper;

        public ItemsController(IItemsService itemsService, IMapper mapper)
        {
            _itemsService = itemsService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ShelfItemListingOutViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllItemsOnShelf()
        {
            try
            {
                var items = await _itemsService.ListAllItems();
                return Ok(_mapper.Map<IEnumerable<ShelfItemListingOutViewModel>>(items));
            }
            catch (EmptyShelfException)
            {
                return NotFound();
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ShelfItemListingOutViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchItemByKeyword([FromQuery][MinLength(2)] string keyword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var items = await _itemsService.ListItemsThatContainKeyword(keyword);
                return Ok(_mapper.Map<IEnumerable<ShelfItemListingOutViewModel>>(items));
            }
            catch (Exception e)
            {
                if (e is NoItemOnShelfSatisfiesConditionException || e is EmptyShelfException)
                    return NotFound();

                throw;
            }
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CompleteShelfItemOutViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetItemFromShelf([FromRoute][Range(0L, long.MaxValue)] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var item = await _itemsService.PopItem(id);
                return Ok(_mapper.Map<CompleteItemDto>(item));
            }
            catch (NoItemOnShelfSatisfiesConditionException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ItemStoredOutViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status507InsufficientStorage)]
        public async Task<IActionResult> StoreItem([FromBody] ItemToStoreInViewModel itemToStore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var itemDto = _mapper.Map<ItemToInsertDto>(itemToStore);
                var newID = await _itemsService.InsertItem(itemDto);
                var returnItem = new ItemStoredOutViewModel
                {
                    Id = newID
                };

                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(returnItem),
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (ShelfFullException)
            {
                var returnObject = new
                {
                    Details = "The shelf is currently full"
                };
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(returnObject),
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status507InsufficientStorage
                };
            }
        }

        [HttpGet("{id:long}/comments")]
        [ProducesResponseType(typeof(IEnumerable<ItemCommentOutViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCommentsForItem([FromRoute][Range(0L, long.MaxValue)] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var comments = await _itemsService.GetCommentsForItem(id);
                return Ok(_mapper.Map<IEnumerable<ItemCommentOutViewModel>>(comments));
            }
            catch (NoItemOnShelfSatisfiesConditionException)
            {
                return NotFound(new
                {
                    Message = "There is no item with the given id",
                    Id = id
                });
            }
            catch (ItemHasNoCommentsException)
            {
                return NotFound(new {
                    Message = "There are no comments for the given item"
                });
            }
        }

        [HttpPost("{id:long}/comments")]
        [ProducesResponseType(typeof(CommentAddedOutViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCommentToItem([FromRoute][Range(0L, long.MaxValue)] long id,
            [FromBody] CommentToAddToItemInViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var commentDto = _mapper.Map<ItemCommentToInsertDto>(comment);
                var newId = await _itemsService.InsertCommentForItem(id, commentDto);
                var returnObject = new CommentAddedOutViewModel
                {
                    Id = newId
                };
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(returnObject),
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (NoItemOnShelfSatisfiesConditionException)
            {
                return NotFound(new
                {
                    Message = "There is no item with the given id",
                    Id = id
                });
            }
        }
    }
}
