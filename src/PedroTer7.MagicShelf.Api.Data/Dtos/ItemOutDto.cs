namespace PedroTer7.MagicShelf.Api.Data.Dtos
{
    public class ItemOutDto
    {
        public long Id { get; set; }
        public DateTime DateStored { get; set; }
        public int NumberCommentsAbout { get; set; }
        public string Description { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
