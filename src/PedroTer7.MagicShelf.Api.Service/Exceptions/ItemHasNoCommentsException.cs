namespace PedroTer7.MagicShelf.Api.Service.Exceptions
{
    public class ItemHasNoCommentsException : Exception
    {
        public ItemHasNoCommentsException(long itemId)
            : base($"Item with id {itemId} has no comments")
        {
        }
    }
}
