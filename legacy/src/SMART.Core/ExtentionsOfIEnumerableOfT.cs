
namespace SMART.Core
{
    using System;
    using System.Collections.Generic;

    public static class ExtentionsOfIEnumerableOfT
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) return;

            foreach (var t in enumerable)
            {
                action(t);
            }
        }
    }
}
