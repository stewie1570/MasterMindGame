using MasterMind.Core.Models;
using System.Linq;

namespace MasterMind.Core.ResultLogic
{
    public class PerPegGuessResultLogic : IGuessResultLogic
    {
        public GuessResult[] ResultFrom(GuessColor[] guess, GuessColor[] actual)
        {
            return guess
                .Zip(actual, (guessPeg, actualPeg) => GuessResultFrom(actual, guessPeg, actualPeg))
                .OrderBy(r => r)
                .ToArray();
        }

        #region Helpers

        private static GuessResult GuessResultFrom(GuessColor[] actual, GuessColor guessPeg, GuessColor actualPeg)
        {
            return guessPeg == actualPeg
                ? GuessResult.Red
                : actual.Contains(guessPeg) ? GuessResult.White : GuessResult.Empty;
        }

        #endregion
    }
}
