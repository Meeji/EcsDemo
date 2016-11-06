namespace PathFinding
{
    using System.Collections.Generic;

    public static class ExtensionMethods
    {
        public static ISet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }
    }
}
