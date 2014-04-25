using MasterMind.Core.Models;
using System.Linq;

namespace MasterMind.Core
{
    public class PerPegGuessResultLogic : IGuessResultLogic
    {
        public GuessResult[] ResultFrom(GuessColor[] guess, GuessColor[] actual)
        {
            return guess
                .Zip(actual, (g, a) => g == a ? GuessResult.Red : actual.Contains(g) ? GuessResult.White : GuessResult.Empty)
                .OrderBy(r => r)
                .ToArray();
        }
    }
}
