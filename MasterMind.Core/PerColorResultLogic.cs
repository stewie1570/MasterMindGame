using MasterMind.Core.Models;
using MasterMind.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core
{
    public class PerColorResultLogic : IGuessResultLogic
    {
        public GuessResult[] ResultFrom(GuessColor[] guess, GuessColor[] actual)
        {
            var colorsInActual = actual.Distinct().ToList();
            var results = new List<GuessResult>();

            colorsInActual.ForEach(color =>
            {
                int countOfColorInActual = actual.Count(ac => ac == color);
                int countOfColorInGuess = guess.Count(gc => gc == color);
                var reds = RedsFor(guess, actual, color).ToList();

                results.AddRange(reds);
                results.AddRange(WhitesFor(countOfColorInActual, countOfColorInGuess, reds));
            });

            if (results.Count < actual.Length)
                results.AddRange(EmptiesFor(actual, results));

            return results.OrderBy(r => r).ToArray();
        }

        #region Helpers

        private IEnumerable<GuessResult> EmptiesFor(GuessColor[] actual, List<GuessResult> results)
        {
            return (actual.Length - results.Count).GuessResults(GuessResult.Empty);
        }

        private IEnumerable<GuessResult> WhitesFor(int countOfColorInActual, int countOfColorInGuess, List<GuessResult> reds)
        {
            return NumOfWhitesFrom(countOfColorInActual, countOfColorInGuess, reds)
                .GuessResults(GuessResult.White);
        }

        private int NumOfWhitesFrom(int countOfColorInActual, int countOfColorInGuess, List<GuessResult> reds)
        {
            return (Math.Min(countOfColorInActual, countOfColorInGuess) - reds.Count);
        }

        private IEnumerable<GuessResult> RedsFor(GuessColor[] guess, GuessColor[] actual, GuessColor color)
        {
            return guess
                .Zip(actual, (g, a) => g == a && a == color ? GuessResult.Red : GuessResult.Empty)
                .Where(r => r == GuessResult.Red);
        }

        #endregion
    }
}
