using MasterMind.Web.Attributes;
using MasterMind.Web.Interfaces;
using System.Web.Mvc;

namespace MasterMind.Web.Controllers.MVC
{
    [JsonHandleErrorAttribute]
    public class HomeController : AutoResultControllerBase
    {
        private IGameController gameController;

        public HomeController(IGameController gameController)
        {
            this.gameController = gameController;
        }

        public ActionResult Index()
        {
            return Result();
        }

        [HttpPost]
        public ActionResult Guess(string guess)
        {
            return Result(gameController.Guess(guess));
        }

        [HttpPost]
        public ActionResult Setup(int width, string resultLogic = "percolor")
        {
            return Result(gameController.Setup(width, resultLogic));
        }
    }
}
