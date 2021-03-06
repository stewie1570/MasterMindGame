﻿using MasterMind.Core.Extensions;
using MasterMind.Core.Models;
using MasterMind.Core.NumberGenerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.ActualProviders
{
    public class RestrictedActualProvider : IActualProvider
    {
        private INumberGenerator numberGenerator;

        public RestrictedActualProvider(INumberGenerator numberGenerator)
        {
            this.numberGenerator = numberGenerator;
        }

        public GuessColor[] Create(int pegCount, int repeatLimit)
        {
            var possibleGuesses = Enum.GetValues(typeof(GuessColor)).Cast<GuessColor>().ToList();
            Validate(pegCount, repeatLimit, possibleGuesses);

            List<GuessColor> colors = new List<GuessColor>();
            Enumerable.Range(start: 1, count: pegCount).ToList().ForEach(i =>
            {
                var index = numberGenerator.GetNumber(minValue: 1, maxValue: possibleGuesses.Count);
                var color = possibleGuesses[index];
                colors.Add(color);
                if (colors.Count(c => c == color) == repeatLimit)
                    possibleGuesses.Remove(color);
            });

            return colors.ToArray();
        }

        #region Helpers

        private void Validate(int pegCount, int repeatLimit, List<GuessColor> possibleGuesses)
        {
            var possibleGuessCount = possibleGuesses.Count - 1;

            if (pegCount < 0 || repeatLimit < 0)
                throw new InvalidOperationException("Peg count and repeat limit must be positive.");

            if (possibleGuessCount < pegCount / repeatLimit)
                throw new InvalidOperationException("{0} peg(s) with only {1} repeating peg(s) requires {2} colors. Only {3} are available."
                    .FormatWith(
                        pegCount,
                        repeatLimit,
                        pegCount / repeatLimit,
                        possibleGuessCount));
        }

        #endregion
    }
}
