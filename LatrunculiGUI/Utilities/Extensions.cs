using System.Collections;

namespace LatrunculiGUI.Utilities
{
    public static class IListExtensions
    {
        public static TReturn GetValueByIndex<TReturn>(this IList source, int index, TReturn defaultValue = default)
        {
            if (source == null || index >= source.Count)
            {
                return defaultValue;
            }
            try
            {
                return (TReturn)(dynamic)(source[index]);
            }
            catch
            {
                return default;
            }
        }
    }
}
