using System.ComponentModel.DataAnnotations;

namespace PedroTer7.MagicShelf.Api.ViewModels.In
{
    public class ItemToStoreInViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "An item must have a description")]
        [MinLength(5, ErrorMessage = "The description must have at least 5 characters")]
        [MaxLength(250, ErrorMessage = "The description must have up to 250 characters")]
        public string Description { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "An item must have a name")]
        [MinLength(3, ErrorMessage = "The name must have at least 3 characters")]
        [MaxLength(100, ErrorMessage = "The name must have up to 150 characters")]
        public string Name { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "An item must have some content")]
        [MaxLength(5000, ErrorMessage = "Cannot store items with more than 5000 chars in content")]
        public string Content { get; set; } = null!; 
    }
}
