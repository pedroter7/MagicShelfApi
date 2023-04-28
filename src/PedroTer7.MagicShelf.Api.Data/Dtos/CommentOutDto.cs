namespace PedroTer7.MagicShelf.Api.Data.Dtos
{
    public class CommentOutDto
    {
        public long Id { get; set; }
        public string Author { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
}
