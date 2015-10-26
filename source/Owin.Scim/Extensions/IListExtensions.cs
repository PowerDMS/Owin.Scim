namespace Owin.Scim.Extensions
{
    using System.Collections;

    public static class IListExtensions
    {
        public static void AddPossibleRange(this IList list, object itemToAdd)
        {
            if (itemToAdd.GetType().IsNonStringEnumerable())
            {
                foreach (var item in (IEnumerable)itemToAdd)
                {
                    list.Add(item);
                }
            }
            else
            {
                list.Add(itemToAdd);
            }
        }
    }
}