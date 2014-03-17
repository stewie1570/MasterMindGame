using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MasterMind.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class JsonHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = new JsonResult
            {
                Data = new { Message = filterContext.Exception.Message },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}