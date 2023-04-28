namespace PedroTer7.MagicShelf.Api.Service.Exceptions
{
    public class NoItemOnShelfSatisfiesConditionException : Exception
    {
        public NoItemOnShelfSatisfiesConditionException(string conditionDescription)
            : base($"There are no items on the shelf that satisfie the following condition: {conditionDescription}")
        {
        }
    }
}
