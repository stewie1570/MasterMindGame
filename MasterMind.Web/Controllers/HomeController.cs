using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Web.Attributes;
using MasterMind.Web.Exceptions;
using MasterMind.Web.ViewModels.Extensions;
using System.Web.Mvc;

namespace MasterMind.Web.Controllers
{
    [JsonHandleErrorAttribute]
    public class HomeController : Controller
    {
        private IGameProcess _gameProcess;
        private IntegerRange _acceptableGuessWidthRange;
        private IntegerRange _acceptableMaxAttemptsRange;

        public HomeController(IGameProcess gameProcess)
        {
            _gameProcess = gameProcess;
            _acceptableGuessWidthRange = new IntegerRange { Min = 2, Max = 10 };
            _acceptableMaxAttemptsRange = new IntegerRange { Min = 2, Max = 25 };
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Guess(string guess)
        {
            return Json(_gameProcess.Guess(guess).AsGuessResultVM(_gameProcess));
        }

        [HttpPost]
        public void Setup(int width)
        {
            Validate(width);
            _gameProcess.Setup(width);
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
