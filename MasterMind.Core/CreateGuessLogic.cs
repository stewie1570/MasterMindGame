using MasterMind.Core.Models;
using System;
using System.Linq;

namespace MasterMind.Core
{
    public static class CreateGuessLogic
    {
        public static Guess[] Create(int count)
        {
            var possibleGuesses = Enum.GetValues(typeof(Guess)).Cast<Guess>().ToList();
            Random random = new Random();

            return Enumerable
                .Range(1, count)
                .Select(i => possibleGuesses[random.Next(1, possibleGuesses.Count)])
                .ToArray();
        }
    }
}
