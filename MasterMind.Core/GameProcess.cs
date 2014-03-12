using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core
{
    public class GameProcess : IGameProcess
    {
        private GuessResultLogic _resultLogic = new GuessResultLogic();
        private Context _context;
        private Func<int, Guess[]> _actualProvider;

        public GameProcess(Func<Context> contextProvider, Func<int, Guess[]> actualProvider)
        {
            _context = contextProvider();
            _actualProvider = actualProvider;
            _context.Results = IsActualInvalidInContext() ? new List<FullGuessResultRow>() : _context.Results;
            _context.Actual = IsActualInvalidInContext() ? _actualProvider(_context.GuessWidth) : _context.Actual;
        }

        public FullGuessResultRow[] Guess(string guessString)
        {
            if (!IsOver)
            {
                var guess = guessString.ToGuessArray(expectedLength: _context.Actual.Length);

                _context.Results.Add(new FullGuessResultRow
                {
                    Result = _resultLogic.ResultFrom(guess, _context.Actual),
                    Guess = guess
                });
            }

            return _context.Results.ToArray();
        }

        public void Setup(int newWidth, int newMaxAttempts)
        {
            _context.MaxAttempts = newMaxAttempts;
            _context.GuessWidth = newWidth;
            _context.Actual = _actualProvider(newWidth);
            _context.Results = new List<FullGuessResultRow>();
        }

        public Guess[] Actual { get { return _context.Actual; } }

        public bool IsOver
        {
            get { return _context.Results.Count >= _context.MaxAttempts || IsAWin; }
        }

        public bool IsAWin
        {
            get { return _context.Results.Any(PerfectGuess()); }
        }

        #region Helpers

        private bool IsActualInvalidInContext()
        {
            return _context.Actual == null || _context.Actual.Length != _context.GuessWidth;
        }

        private static Func<FullGuessResultRow, bool> PerfectGuess()
        {
            return resultArray => resultArray.Result.All(r => r == GuessResult.Red);
        }

        #endregion
    }
}
