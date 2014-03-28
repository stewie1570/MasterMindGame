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
            int colorCount = gameProcess.Actual.Distinct().Count();
            var totalTimeLaps = TotalTimeSpanFrom(gameContext);

            return new GuessResultVM
            {
                Results = results,
                IsOver = gameProcess.IsOver,
                IsAWin = gameProcess.IsAWin,
                MaxAttempts = gameContext.MaxAttempts,
                Actual = gameProcess.IsOver ? gameContext.Actual : null,
                TotalTimeLapse = gameProcess.IsOver ? totalTimeLaps : TimeSpan.FromTicks(0),
                ColorCount = gameProcess.IsOver ? gameProcess.Actual.Distinct().Count() : (int?)null,
                Score = gameProcess.IsOver ? CalculateScoreFrom(colorCount, totalTimeLaps, gameProcess.Actual.Length) : (int?)null
            };
        }

        #region Helpers

        private static int CalculateScoreFrom(int colorCount, TimeSpan totalTimeLaps, int actualWidth)
        {
            return (int)((1 / (0.001 * totalTimeLaps.TotalSeconds)) * (double)colorCount * ((double)actualWidth / 2));
        }

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