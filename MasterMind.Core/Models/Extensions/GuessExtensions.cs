﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.Models.Extensions
{
    public static class GuessExtensions
    {
        private static Dictionary<Guess, char> _map = new Dictionary<Guess, char>
        {
            {Guess.Blue, 'b'},
            {Guess.Empty, 'e'},
            {Guess.Green, 'g'},
            {Guess.Purple, 'p'},
            {Guess.Red, 'r'},
            {Guess.Yellow, 'y'}
        };

        public static string ToGuessString(this IEnumerable<Guess> guessList)
        {
            return new String(guessList.Select(g => _map[g]).ToArray());
        }

        public static Guess[] ToGuessArray(this string guess, int? expectedLength = null)
        {
            ValidateGuess(guess, expectedLength);

            return guess
                .ToLower()
                .ToCharArray()
                .Select(c => _map.First(m => m.Value == c).Key)
                .ToArray();
        }

        #region Helpers

        private static void ValidateGuess(string guess, int? expectedLength)
        {
            char[] guessChars = guess.ToLower().ToCharArray();

            if(expectedLength != null && guessChars.Length != expectedLength)
                throw new InvalidGuessException(string.Format("The guess \"{0}\" has a length of {1}. Length should be {2}.",
                    guess,
                    guess.Length,
                    expectedLength));

            var invalidChars = guessChars
                .Where(c => !_map.Select(m => m.Value).Contains(c))
                .ToList();

            if (invalidChars.Count > 0)
                throw new InvalidGuessException(string.Format("The following colors ({0}) do not exist in this game.",
                    string.Join(", ", invalidChars)));
        }

        #endregion
    }

    public class InvalidGuessException : Exception
    {
        public InvalidGuessException(string message, Exception inner) : base(message, inner) { }
        public InvalidGuessException(string message) : base(message) { }
    }
}
