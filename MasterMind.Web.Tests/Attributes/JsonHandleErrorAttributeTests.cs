using FizzWare.NBuilder;
using FluentAssertions;
using MasterMind.Web.Attributes;
using MasterMind.Web.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MasterMind.Web.Tests.Attributes
{
    [TestClass]
    public class JsonHandleErrorAttributeTests
    {
        [TestMethod]
        public void OnExceptionShouldEditContextAccordingly()
        {
            //Arrange
            HandleErrorAttribute attribute = new JsonHandleErrorAttribute();
            var context = Builder<ExceptionContext>.CreateNew().Build();
            context.Exception = Builder<Exception>.CreateNew().Build();
            context.HttpContext = Substitute.For<HttpContextBase>();

            //Act
            attribute.OnException(context);

            //Assert
            ((context.Result as JsonResult).Data as ExceptionVM).Message.Should().Be(context.Exception.Message);
            ((context.Result) as JsonResult).JsonRequestBehavior.Should().Be(JsonRequestBehavior.AllowGet);
            context.ExceptionHandled.Should().BeTrue();
            context.HttpContext.Response.Received().StatusCode = (int)HttpStatusCode.OK;
            context.HttpContext.Response.Cache.Received(1).SetCacheability(Arg.Is(HttpCacheability.NoCache));
        }
    }
}
