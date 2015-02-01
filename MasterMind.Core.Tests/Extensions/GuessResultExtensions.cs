using MasterMind.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.Tests.Extensions
{
    public static class GuessResultExtensions
    {
        public static string AsResultString(this IEnumerable<GuessResult> results)
        {
            return new String(results.Select(r => ResultToChar(r)).ToArray());
        }

        private static char ResultToChar(GuessResult result)
        {
            switch (result)
            {
                case GuessResult.Red: return 'r';
                case GuessResult.White: return 'w';
                case GuessResult.Empty: return 'e';
                default: return ' ';
            }
        }
    }
}
