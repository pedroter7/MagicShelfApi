using System.ComponentModel.DataAnnotations;

namespace PedroTer7.MagicShelf.Api.ViewModels.In
{
    public class CommentToAddToItemInViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Comment author name must have at least 3 chars")]
        [MaxLength(120, ErrorMessage = "Comment author name must have up to 120 chars")]
        public string Author { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "Comment text must have at least 3 chars")]
        [MaxLength(250, ErrorMessage = "Comment text must have up to 250 chars")]
        public string Text { get; set; } = null!;
    }
}
