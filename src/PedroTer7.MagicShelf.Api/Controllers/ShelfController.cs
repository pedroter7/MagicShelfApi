using Microsoft.AspNetCore.Mvc;
using PedroTer7.MagicShelf.Api.Service.Services.Interfaces;
using PedroTer7.MagicShelf.Api.ViewModels.Out;

namespace PedroTer7.MagicShelf.Api.Controllers
{
    [ApiController]
    [Route("api/v1/shelf")]
    public class ShelfController : Controller
    {
        private readonly IItemsService _itemsService;

        public ShelfController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        [HttpGet("status")]
        [ProducesResponseType(typeof(ShelfStatusOutViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShelfStatus()
        {
            var hasRoom = await _itemsService.ThereIsRoomForNewItems();
            return Ok(new ShelfStatusOutViewModel
            {
                ThereIsRoomForNewItem = hasRoom
            });
        }
    }
}
