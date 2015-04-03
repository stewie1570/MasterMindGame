using FizzWare.NBuilder;
using FluentAssertions;
using MasterMind.Web.Controllers.MVC;
using MasterMind.Web.Interfaces;
using MasterMind.Web.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web.Mvc;

namespace MasterMind.Web.Tests.Controllers.MVC
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController controller;
        private IGameController gameController;

        [TestInitialize]
        public void Setup()
        {
            gameController = Substitute.For<IGameController>();
            controller = new HomeController(gameController);
        }

        [TestMethod]
        public void IndexShouldReturnView()
        {
            //Arrange
            //Act
            //Assert
            controller.Index().Should().BeAssignableTo<ViewResult>();
        }

        [TestMethod]
        public void GuessMethodShouldReturnProperViewModelFromGuess()
        {
            //Arrang
            var expectedViewModel = Builder<GuessResultVM>.CreateNew().Build();
            gameController.Guess(guess: "expected guess").Returns(expectedViewModel);

            //Act
            var result = controller.Guess(guess: "expected guess") as JsonResult;

            //Assert
            result.Data.Should().Be(expectedViewModel);
        }

        [TestMethod]
        public void SetupMethodShouldReturnProperViewModelFromSetupConfig()
        {
            //Arrang
            var expectedViewModel = Builder<GuessResultVM>.CreateNew().Build();
            gameController.Setup(width: 5, resultLogic: "percolor").Returns(expectedViewModel);

            //Act
            var result = controller.Setup(width: 5, resultLogic: "percolor") as JsonResult;

            //Assert
            result.Data.Should().Be(expectedViewModel);
        }
    }
}
