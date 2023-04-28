namespace PedroTer7.MagicShelf.Api.ViewModels.Out
{
    public class ItemCommentOutViewModel
    {
        public long Id { get; set; }
        public string Author { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime PostedDate { get; set; }
    }
}
