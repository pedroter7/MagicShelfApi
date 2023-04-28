namespace PedroTer7.MagicShelf.Api.Data.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string name, string identifier)
            : base($"Could not find an entity for {name} using as identifier: {identifier}")
        {

        }
    }
}
