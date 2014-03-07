using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core
{
    public class GameProcess
    {
        private Guess[] _actual;
        private List<FullGuestResultRow> _results;
        private GuessResultLogic _resultLogic = new GuessResultLogic();
        private int _maxAttempts;

        public GameProcess(int maxAttempts, int guessWidth, Func<int, Guess[]> actualProvider)
        {
            _actual = actualProvider(guessWidth);
            _results = new List<FullGuestResultRow>();
            _maxAttempts = maxAttempts;
        }

        public FullGuestResultRow[] Guess(string guessString)
        {
            if (!IsOver)
            {
                var guess = guessString.ToGuessArray(expectedLength: _actual.Length);

                _results.Add(new FullGuestResultRow
                {
                    Result = _resultLogic.ResultFrom(guess, _actual),
                    Guess = guess
                });
            }

            return _results.ToArray();
        }

        public Guess[] Actual { get { return _actual; } }

        public bool IsOver
        {
            get { return _results.Count >= _maxAttempts || IsAWin; }
        }

        public bool IsAWin
        {
            get { return _results.Any(PerfectGuess()); }
        }

        #region Helpers

        private static Func<FullGuestResultRow, bool> PerfectGuess()
        {
            return resultArray => resultArray.Result.All(r => r == GuessResult.Red);
        }

        #endregion
    }
}
