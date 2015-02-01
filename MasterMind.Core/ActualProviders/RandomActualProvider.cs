using MasterMind.Core.Models;
using System;
using System.Linq;

namespace MasterMind.Core.ActualProviders
{
    public static class RandomActualProvider
    {
        public static GuessColor[] Create(int count)
        {
            var possibleGuesses = Enum.GetValues(typeof(GuessColor)).Cast<GuessColor>().ToList();
            Random random = new Random();

            return Enumerable
                .Range(1, count)
                .Select(i => possibleGuesses[random.Next(1, possibleGuesses.Count)])
                .ToArray();
        }
    }
}
