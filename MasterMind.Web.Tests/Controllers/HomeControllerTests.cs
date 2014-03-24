using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Web.Controllers;
using MasterMind.Web.ViewModels;
using MasterMind.Web.Exceptions;
using System;
using System.Web.Mvc;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NSubstitute;
using FizzWare.NBuilder;

namespace MasterMind.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private IGameProcess _fakeGameProcess;
        private Context _gameContext;

        [TestInitialize]
        public void Setup()
        {
            _gameContext = new Context();
            _fakeGameProcess = Substitute.For<IGameProcess>();
            _controller = new HomeController(_fakeGameProcess, () => _gameContext);
        }

        [TestMethod]
        public void ShouldFailSetupIfParamsAreNotWithInDefaultRange()
        {
            //Arrange
            //Act
            Action invalidWidth = () => _controller.Setup(width: 600);

            //Assert
            invalidWidth.ShouldThrow<InvalidRequestException>()
                .WithMessage("Guess width of 600 is not between 2 and 10.");
        }

        [TestMethod]
        public void IndexShouldReturnView()
        {
            //Arrange
            //Act
            //Assert
            _controller.Index().Should().BeAssignableTo<ViewResult>();
        }

        [TestMethod]
        public void SetupShouldSetUpGameContextViaGameProcessObject()
        {
            //Arrange
            //Act
            _controller.Setup(6);

            //Assert
            _fakeGameProcess.Received(1).Setup(newWidth: 6);
        }

        [TestMethod]
        public void SetupShouldReturnAnEmptyResultVM()
        {
            //Arrange
            _gameContext.MaxAttempts = 12;

            //Act
            var results = _controller.Setup(6).Data;

            //Assert
            results.Should().BeAssignableTo<GuessResultVM>();
            (results as GuessResultVM).MaxAttempts.Should().Be(12);
        }

        [TestMethod]
        public void GuessShouldReturnGuessResultsToClient()
        {
            //Arrange
            _gameContext.MaxAttempts = 12;
            var expectedResults = Builder<FullGuessResultRow>.CreateListOfSize(2).Build().ToArray();
            _fakeGameProcess.Guess(Arg.Any<string>()).Returns(expectedResults);
            _fakeGameProcess.IsAWin.Returns(true);
            _fakeGameProcess.IsOver.Returns(true);

            //Act
            var vm = _controller.Guess(string.Empty).Data as GuessResultVM;

            //Assert
            vm.Should().BeAssignableTo<GuessResultVM>();
            vm.Results.Should().BeEquivalentTo(expectedResults);
            vm.IsAWin.Should().BeTrue();
            vm.IsOver.Should().BeTrue();
            vm.MaxAttempts.Should().Be(12);
        }
    }
}
