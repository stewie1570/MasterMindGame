using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.Extensions
{
    public static class CoreExtensions
    {
        public static IEnumerable<T> CopiesOf<T>(this int num, T result)
        {
            return Enumerable.Range(0, num).Select(i => result);
        }
    }
}
