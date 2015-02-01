using MasterMind.Core.Extensions;
using MasterMind.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.ActualProviders
{
    public class RestrictedRandomActualProvider : IActualProvider
    {
        private INumberGenerator numberGenerator;

        public RestrictedRandomActualProvider(INumberGenerator numberGenerator)
        {
            this.numberGenerator = numberGenerator;
        }

        public GuessColor[] Create(int pegCount, int repeatLimit)
        {
            if (pegCount < 0 || repeatLimit < 0)
                throw new InvalidOperationException("Peg count and repeat limit must be positive.");

            var possibleGuesses = Enum.GetValues(typeof(GuessColor)).Cast<GuessColor>().ToList();
            var possibleGuessCount = possibleGuesses.Count - 1;

            if (possibleGuessCount < pegCount / repeatLimit)
                throw new InvalidOperationException("{0} peg(s) with only {1} repeating peg(s) requires {2} colors. Only {3} are available."
                    .FormatWith(
                        pegCount,
                        repeatLimit,
                        pegCount / repeatLimit,
                        possibleGuessCount));

            List<GuessColor> colors = new List<GuessColor>();
            Enumerable.Range(start: 1, count: pegCount).ToList().ForEach(i =>
            {
                var index = numberGenerator.GetNumber(minValue: 1, maxValue: possibleGuesses.Count - 1);
                var color = possibleGuesses[index];
                colors.Add(color);
                if (colors.Count(c => c == color) == repeatLimit)
                    possibleGuesses.Remove(color);
            });

            return colors.ToArray();
        }
    }
}
