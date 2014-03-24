using MasterMind.Core;
using MasterMind.Core.Models;

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
                MaxAttempts = gameContext.MaxAttempts
            };
        }
    }
}