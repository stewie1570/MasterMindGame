using System.Web.Mvc;

namespace MasterMind.Web.Controllers
{
    public abstract class AutoResultControllerBase : Controller
    {
        protected ActionResult Result(object obj)
        {
            return Request == null || Request.IsAjaxRequest() ? (ActionResult)Json(obj) : (ActionResult)View(obj);
        }

        protected ActionResult Result() { return View(); }
    }
}
