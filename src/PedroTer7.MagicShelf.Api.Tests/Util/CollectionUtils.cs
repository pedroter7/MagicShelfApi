using Bogus;

namespace PedroTer7.MagicShelf.Api.Tests.Util
{
    internal static class CollectionUtils
    {
        internal static int ApplyModificationToSomeItemsInCollection<T>(IList<T> collection, Action<T> modifyItem)
        {
            var faker = new Faker();
            var alreadyModified = new List<int>();
            var itemsToModify = faker.Random.Int(1, collection.Count - 1);
            for (var i = 0; i < itemsToModify; i++)
            {
                int idx;
                do
                {
                    idx = faker.Random.Int(0, collection.Count - 1);
                } while (alreadyModified.Contains(idx));
                alreadyModified.Add(idx);
                modifyItem.Invoke(collection[idx]);
            }
            return itemsToModify;
        }
    }
}
