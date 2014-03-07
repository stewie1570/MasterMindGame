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
        private List<FullGuessResultRow> _results;
        private GuessResultLogic _resultLogic = new GuessResultLogic();
        private int _maxAttempts;

        public GameProcess(Func<Context> contextProvider, Func<int, Guess[]> actualProvider)
        {
            var context = contextProvider();
            _actual = context.Actual ?? actualProvider(context.GuessWidth);
            _results = context.Results ?? new List<FullGuessResultRow>();
            _maxAttempts = context.MaxAttempts;
        }

        public FullGuessResultRow[] Guess(string guessString)
        {
            if (!IsOver)
            {
                var guess = guessString.ToGuessArray(expectedLength: _actual.Length);

                _results.Add(new FullGuessResultRow
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

        private static Func<FullGuessResultRow, bool> PerfectGuess()
        {
            return resultArray => resultArray.Result.All(r => r == GuessResult.Red);
        }

        #endregion
    }
}
