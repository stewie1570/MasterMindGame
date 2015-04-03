using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using MasterMind.Web.Exceptions;
using MasterMind.Web.Interfaces;
using MasterMind.Web.ViewModels;
using MasterMind.Web.ViewModels.Extensions;
using System;

namespace MasterMind.Web.Controllers
{
    public class GameController : IGameController
    {
        private IGameProcess gameProcess;
        private Func<Context> contextProvider;
        private IntegerRange acceptableGuessWidthRange = new IntegerRange { Min = 2, Max = 10 };
        private IntegerRange acceptableMaxAttemptsRange = new IntegerRange { Min = 2, Max = 25 };

        public GameController(IGameProcess gameProcess, Func<Context> contextProvider)
        {
            this.gameProcess = gameProcess;
            this.contextProvider = contextProvider;
        }

        public GuessResultVM Guess(string guess)
        {
            return gameProcess.Guess(guess).AsGuessResultVM(gameProcess, contextProvider());
        }

        public GuessResultVM Setup(int width, string resultLogic = "percolor")
        {
            Validate(width);
            gameProcess.Setup(
                newWidth: width,
                logicType: resultLogic.ToGuessResultLogicType());

            return new GuessResultVM { MaxAttempts = contextProvider().MaxAttempts };
        }

        #region Helpers

        private void Validate(int width)
        {
            ThrowIfNotWithInRange(width, acceptableGuessWidthRange, "Guess width");
        }

        private void ThrowIfNotWithInRange(int x, IntegerRange range, string rangeDescription)
        {
            if (x > range.Max || x < range.Min)
                throw new InvalidRequestException(string.Format("{3} of {0} is not between {1} and {2}.",
                    x,
                    range.Min,
                    range.Max,
                    rangeDescription));
        }

        #endregion
    }
}