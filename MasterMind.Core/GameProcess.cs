using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core
{
    public class GameProcess : IGameProcess
    {
        private IGuessResultLogic _resultLogic;
        private Context _context;
        private Func<int, GuessColor[]> _actualProvider;
        private Func<DateTime> _timeProvider;

        public GameProcess(Func<Context> contextProvider, Func<int, GuessColor[]> actualProvider)
            : this(contextProvider, actualProvider, () => DateTime.UtcNow) { }

        public GameProcess(Func<Context> contextProvider,
           Func<int, GuessColor[]> actualProvider,
           Func<DateTime> timeProvider)
            : this(contextProvider, actualProvider, timeProvider, new PerPegGuessResultLogic()) { }

        public GameProcess(Func<Context> contextProvider,
            Func<int, GuessColor[]> actualProvider,
            Func<DateTime> timeProvider,
            IGuessResultLogic resultLogic)
        {
            _context = contextProvider();
            _timeProvider = timeProvider;
            _actualProvider = actualProvider;
            _resultLogic = resultLogic;

            bool isActualInvalidInContext = IsActualInvalidInContext();
            _context.Results = isActualInvalidInContext ? new List<FullGuessResultRow>() : _context.Results;
            _context.Actual = isActualInvalidInContext ? _actualProvider(_context.GuessWidth) : _context.Actual;
        }

        public FullGuessResultRow[] Guess(string guessString)
        {
            if (!IsOver)
            {
                var guess = guessString.ToGuessArray(expectedLength: _context.Actual.Length);
                var currentTime = _timeProvider();

                _context.Results.Add(new FullGuessResultRow
                {
                    Result = _resultLogic.ResultFrom(guess, _context.Actual),
                    Guess = guess,
                    TimeStamp = currentTime,
                    TimeLapse = TimeLapseFrom(currentTime)
                });
            }

            return _context.Results.ToArray();
        }

        public void Setup(int newWidth)
        {
            _context.MaxAttempts = 10 + ((newWidth - 4) * 3);
            _context.GuessWidth = newWidth;
            _context.Actual = _actualProvider(newWidth);
            _context.Results = new List<FullGuessResultRow>();
        }

        public GuessColor[] Actual { get { return _context.Actual; } }

        public bool IsOver
        {
            get { return _context.Results.Count >= _context.MaxAttempts || IsAWin; }
        }

        public bool IsAWin
        {
            get { return _context.Results.Any(PerfectGuess()); }
        }

        #region Helpers

        private TimeSpan TimeLapseFrom(DateTime currentTime)
        {
            return _context.Results.Count > 0 ? currentTime - _context.Results.Last().TimeStamp : TimeSpan.FromTicks(0);
        }

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
