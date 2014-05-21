using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Web.Attributes;
using MasterMind.Web.Exceptions;
using MasterMind.Web.ViewModels;
using MasterMind.Web.ViewModels.Extensions;
using System;
using System.Web.Mvc;

namespace MasterMind.Web.Controllers
{
    [JsonHandleErrorAttribute]
    public class HomeController : AutoResultControllerBase
    {
        private IGameProcess _gameProcess;
        private Func<Context> _contextProvider;
        private IntegerRange _acceptableGuessWidthRange = new IntegerRange { Min = 2, Max = 10 };
        private IntegerRange _acceptableMaxAttemptsRange = new IntegerRange { Min = 2, Max = 25 };

        public HomeController(IGameProcess gameProcess, Func<Context> contextProvider)
        {
            _gameProcess = gameProcess;
            _contextProvider = contextProvider;
        }

        public ActionResult Index()
        {
            return Result();
        }

        [HttpPost]
        public ActionResult Guess(string guess)
        {
            return Result(_gameProcess.Guess(guess).AsGuessResultVM(_gameProcess, _contextProvider()));
        }

        [HttpPost]
        public ActionResult Setup(int width, string resultLogic = "percolor")
        {
            Validate(width);
            _gameProcess.Setup(
                newWidth: width,
                logicType: resultLogic.ToLower().Contains("peg")
                    ? GuessResultLogicType.PerPeg
                    : GuessResultLogicType.PerColor);

            return Result(new GuessResultVM { MaxAttempts = _contextProvider().MaxAttempts });
        }

        #region Helpers

        private void Validate(int width)
        {
            ThrowIfNotWithInRange(width, _acceptableGuessWidthRange, "Guess width");
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
