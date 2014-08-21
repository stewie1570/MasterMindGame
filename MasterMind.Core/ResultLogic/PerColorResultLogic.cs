using MasterMind.Core.Models;
using MasterMind.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.ResultLogic
{
    public class PerColorResultLogic : IGuessResultLogic
    {
        public GuessResult[] ResultFrom(GuessColor[] guess, GuessColor[] actual)
        {
            var colorsInActual = actual.Distinct().ToList();
            var results = new List<GuessResult>();

            colorsInActual.ForEach(color => results.AddRange(RedsAndWhitesFor(guess, actual, color)));

            if (results.Count < actual.Length)
                results.AddRange(EmptiesFor(actual, results));

            return results.OrderBy(r => r).ToArray();
        }

        #region Helpers

        private List<GuessResult> RedsAndWhitesFor(GuessColor[] guess, GuessColor[] actual, GuessColor color)
        {
            int countOfColorInActual = actual.Count(ac => ac == color);
            int countOfColorInGuess = guess.Count(gc => gc == color);
            var reds = RedsFor(guess, actual, color).ToList();

            return WhitesFor(countOfColorInActual, countOfColorInGuess, reds)
                .Concat(reds)
                .ToList();
        }

        private IEnumerable<GuessResult> EmptiesFor(GuessColor[] actual, List<GuessResult> results)
        {
            return (actual.Length - results.Count).CopiesOf(GuessResult.Empty);
        }

        private IEnumerable<GuessResult> WhitesFor(int countOfColorInActual, int countOfColorInGuess, List<GuessResult> reds)
        {
            return NumOfWhitesFrom(countOfColorInActual, countOfColorInGuess, reds).CopiesOf(GuessResult.White);
        }

        private int NumOfWhitesFrom(int countOfColorInActual, int countOfColorInGuess, List<GuessResult> reds)
        {
            return (Math.Min(countOfColorInActual, countOfColorInGuess) - reds.Count);
        }

        private IEnumerable<GuessResult> RedsFor(GuessColor[] guess, GuessColor[] actual, GuessColor color)
        {
            return guess
                .Zip(actual, (guessPeg, actualPeg) => GuessResultFrom(color, guessPeg, actualPeg))
                .Where(r => r == GuessResult.Red);
        }

        private static GuessResult GuessResultFrom(GuessColor color, GuessColor guessPeg, GuessColor actualPeg)
        {
            return guessPeg == actualPeg && actualPeg == color ? GuessResult.Red : GuessResult.Empty;
        }

        #endregion
    }
}
