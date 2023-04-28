using System.ComponentModel.DataAnnotations;

namespace PedroTer7.MagicShelf.Api.Data.Entities
{
    public class EntityBase
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
