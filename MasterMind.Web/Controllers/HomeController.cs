using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Web.ViewModels.Extensions;
using System;
using System.Web.Mvc;

namespace MasterMind.Web.Controllers
{
    public class HomeController : Controller
    {
        private IGameProcess _gameProcess;
        private Func<Context> _contextProvider;

        public HomeController(IGameProcess gameProcess, Func<Context> contextProvider)
        {
            _gameProcess = gameProcess;
            _contextProvider = contextProvider;
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
        public void Setup(int width, int maxAttempts)
        {
            _gameProcess.Setup(newWidth: width, newMaxAttempts: maxAttempts);
        }
    }
}
