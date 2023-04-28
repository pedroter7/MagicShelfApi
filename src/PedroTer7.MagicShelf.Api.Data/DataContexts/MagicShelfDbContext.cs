using Microsoft.EntityFrameworkCore;
using PedroTer7.MagicShelf.Api.Data.Entities;

namespace PedroTer7.MagicShelf.Api.Data.DataContexts
{
    public class MagicShelfDbContext : DbContext
    {
        public MagicShelfDbContext(DbContextOptions<MagicShelfDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
}
