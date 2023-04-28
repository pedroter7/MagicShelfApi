namespace PedroTer7.MagicShelf.Api.ViewModels.Out
{
    public class CompleteShelfItemOutViewModel
    {
        public long Id { get; set; }
        public DateTime DateStored { get; set; }
        public int NumberCommentsAbout { get; set; }
        public string Description { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
