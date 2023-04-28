using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedroTer7.MagicShelf.Api.Data.Entities
{
    [Table("Comments")]
    public class Comment : EntityBase
    {
        [ForeignKey(nameof(Item))]
        public long ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public string Author { get; set; } = null!;

        [MaxLength(250)]
        public string Details { get; set; } = null!;
    }
}
