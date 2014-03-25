using MasterMind.Core;
using MasterMind.Core.Models;
using System;
using System.Linq;

namespace MasterMind.Web.ViewModels.Extensions
{
    public static class GuessResultVMExtensions
    {
        public static GuessResultVM AsGuessResultVM(this FullGuessResultRow[] results, IGameProcess gameProcess, Context gameContext)
        {
            return new GuessResultVM
            {
                Results = results,
                IsOver = gameProcess.IsOver,
                IsAWin = gameProcess.IsAWin,
                MaxAttempts = gameContext.MaxAttempts,
                Actual = gameProcess.IsOver ? gameContext.Actual : null,
                TotalTimeLapse = gameProcess.IsOver ? TotalTimeSpanFrom(gameContext) : TimeSpan.FromTicks(0)
            };
        }

        #region Helpers

        private static TimeSpan TotalTimeSpanFrom(Context gameContext)
        {
            return (gameContext.Results == null)
                ? TimeSpan.FromTicks(0)
                : (gameContext.Results.Count >= 2)
                    ? gameContext.Results.Last().TimeStamp - gameContext.Results.First().TimeStamp
                    : TimeSpan.FromTicks(0);
        }

        #endregion
    }
}