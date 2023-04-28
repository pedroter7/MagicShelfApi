namespace PedroTer7.MagicShelf.Api.Service.Exceptions
{
    public class ShelfFullException : Exception
    {
        public ShelfFullException() : base("The shelf is full")
        {
        }
    }
}
