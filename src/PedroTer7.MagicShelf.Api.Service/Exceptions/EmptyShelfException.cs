namespace PedroTer7.MagicShelf.Api.Service.Exceptions
{
    public class EmptyShelfException : Exception
    {
        public EmptyShelfException() : base("The shelf is empty")
        {
        }
    }
}
