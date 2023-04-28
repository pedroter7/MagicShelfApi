using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedroTer7.MagicShelf.Api.Data.Entities
{
    [Table("Items")]
    public class Item : EntityBase
    {
        [MaxLength(5000, ErrorMessage = "The item is too big for the shelf")]
        public string Content { get; set; } = null!;

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(250)]
        public string Description { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
    }
}
