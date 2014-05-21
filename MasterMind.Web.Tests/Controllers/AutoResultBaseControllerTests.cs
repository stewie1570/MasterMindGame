﻿using System.Web;
using System.Net;
using System.Web.Routing;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterMind.Web.Controllers;
using FluentAssertions;
using NSubstitute;

namespace MasterMind.Web.Tests.Controllers
{
    public class TestController : AutoResultControllerBase
    {
        public ActionResult Index()
        {
            return Result(new { something = "it worked..." });
        }

        public ActionResult Empty()
        {
            return Result();
        }
    }

    [TestClass]
    public class AutoResultBaseControllerTests
    {
        private TestController controller;

        [TestInitialize]
        public void Setup()
        {
            controller = new TestController();
        }

        [TestMethod]
        public void ShouldReturnJsonResultWithCorrectData()
        {
            //Arrange
            SetupAjaxRequestInControllerContext();

            //Act
            var result = controller.Index() as JsonResult;

            //Assert
            result.Should().BeAssignableTo<JsonResult>();
            ((result.Data as dynamic).something as string).Should().Be("it worked...");
        }

        [TestMethod]
        public void ShouldReturnViewResultWithCorrectViewModelData()
        {
            //Arrange
            SetupDefaultControllerContext();

            //Act
            var result = controller.Index() as ViewResult;

            //Assert
            result.Should().BeAssignableTo<ViewResult>();
            ((result.Model as dynamic).something as string).Should().Be("it worked...");
        }

        [TestMethod]
        public void ShouldDefaultToJsonResultWhenRequestIsNull()
        {
            //Arrange
            //Act
            //Assert
            controller.Index().Should().BeAssignableTo<JsonResult>();
        }

        [TestMethod]
        public void ShouldDefaultToViewResultWhenNoParamsArePassed()
        {
            //Arrange
            //Act
            //Assert
            controller.Empty().Should().BeAssignableTo<ViewResult>();
        }

        #region Helpers

        private void SetupDefaultControllerContext()
        {
            var context = Substitute.For<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);
        }

        private void SetupAjaxRequestInControllerContext()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.Headers.Returns(new WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
            var context = Substitute.For<HttpContextBase>();
            context.Request.Returns(request);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);
        }

        #endregion
    }
}
