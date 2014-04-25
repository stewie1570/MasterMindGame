using MasterMind.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.Extensions
{
    public static class CoreExtensions
    {
        public static IEnumerable<GuessResult> GuessResults(this int num, GuessResult result)
        {
            return Enumerable.Range(0, num).Select(i => result);
        }
    }
}
