namespace PedroTer7.MagicShelf.Api.Service.Dtos
{
    public class ItemCommentDto
    {
        public long Id { get; set; }
        public string Author { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
}
