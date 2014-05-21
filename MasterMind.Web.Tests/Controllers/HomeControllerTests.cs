using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using MasterMind.Web.Controllers;
using MasterMind.Web.ViewModels;
using MasterMind.Web.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
        public void SetupShouldDefaultResultLogicToPerColor()
        {
            //Arrange
            //Act
            _controller.Setup(6);

            //Assert
            _fakeGameProcess.Received(1).Setup(newWidth: 6, logicType: GuessResultLogicType.PerColor);
        }

        [TestMethod]
        public void SetupShouldUsePerPegWhenStringContainsPeg()
        {
            //Arrange
            //Act
            _controller.Setup(6, "143peg233245");

            //Assert
            _fakeGameProcess.Received(1).Setup(newWidth: 6, logicType: GuessResultLogicType.PerPeg);
        }

        [TestMethod]
        public void SetupShouldSetUpGameContextViaGameProcessObject()
        {
            //Arrange
            //Act
            _controller.Setup(6);

            //Assert
            _fakeGameProcess.Received(1).Setup(newWidth: 6, logicType: Arg.Any<GuessResultLogicType>());
        }

        [TestMethod]
        public void SetupShouldReturnAnEmptyResultVM()
        {
            //Arrange
            _gameContext.MaxAttempts = 12;

            //Act
            var results = (_controller.Setup(6) as JsonResult).Data;

            //Assert
            results.Should().BeAssignableTo<GuessResultVM>();
            (results as GuessResultVM).MaxAttempts.Should().Be(12);
        }

        [TestMethod]
        public void ShouldShowActualAndTotalTimeWhenGameIsOver()
        {
            //Arrange
            _fakeGameProcess.IsOver.Returns(true);
            _gameContext.Actual = "rrrr".ToGuessArray();
            _gameContext.Results = new List<FullGuessResultRow>();
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:00 am") });
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:05 am") });

            //Act
            var results = (_controller.Guess("bbbb") as JsonResult).Data as GuessResultVM;

            //Assert
            results.Actual.Should().BeEquivalentTo("rrrr".ToGuessArray());
            results.TotalTimeLapse.Should().Be(TimeSpan.FromMinutes(5));
        }

        [TestMethod]
        public void CertainFieldsShouldNotBePresentWhenGameIsNotOver()
        {
            //Arrange
            _fakeGameProcess.IsOver.Returns(false);
            _gameContext.Actual = "rrrr".ToGuessArray();
            _gameContext.Results = new List<FullGuessResultRow>();
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:00 am") });
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:05 am") });

            //Act
            var results = (_controller.Guess("bbbb") as JsonResult).Data as GuessResultVM;

            //Assert
            results.Actual.Should().BeNull();
            results.TotalTimeLapse.Should().Be(TimeSpan.FromMinutes(0));
            results.ColorCount.Should().Be(null);
            results.Score.Should().Be(null);
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
            var vm = (_controller.Guess(string.Empty) as JsonResult).Data as GuessResultVM;

            //Assert
            vm.Should().BeAssignableTo<GuessResultVM>();
            vm.Results.Should().BeEquivalentTo(expectedResults);
            vm.IsAWin.Should().BeTrue();
            vm.IsOver.Should().BeTrue();
            vm.MaxAttempts.Should().Be(12);
        }

        [TestMethod]
        public void FinalResultShouldIncludeColorCount()
        {
            //Arrange
            _fakeGameProcess.Actual.Returns("rbry".ToGuessArray());
            _fakeGameProcess.IsOver.Returns(true);

            //Act
            var results = (_controller.Guess("rbry") as JsonResult).Data as GuessResultVM;

            //Assert
            results.ColorCount.Should().Be(3);
        }

        [TestMethod]
        public void FinalResultShouldIncludeScore()
        {
            //Arrange
            _fakeGameProcess.Actual.Returns("rbry".ToGuessArray());
            _fakeGameProcess.IsOver.Returns(true);
            _gameContext.Results = new List<FullGuessResultRow>();
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 1:00:00 pm") });
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 1:00:50 pm") });

            //Act
            var results = (_controller.Guess("rbry") as JsonResult).Data as GuessResultVM;

            //Assert
            results.Score.Should().Be(96);
        }

        [TestMethod]
        public void ZeroTimeSpanShouldNotCrashScoring()
        {
            //Arrange
            _fakeGameProcess.Actual.Returns("rbry".ToGuessArray());
            _fakeGameProcess.IsOver.Returns(true);
            _gameContext.Results = new List<FullGuessResultRow>();

            //Act
            var results = (_controller.Guess("rbry") as JsonResult).Data as GuessResultVM;

            //Assert
            results.Score.Should().Be(_fakeGameProcess.Actual.Length * 100);
        }
    }
}
