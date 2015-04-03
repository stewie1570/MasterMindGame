using FizzWare.NBuilder;
using FluentAssertions;
using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using MasterMind.Web.Controllers;
using MasterMind.Web.Exceptions;
using MasterMind.Web.Interfaces;
using MasterMind.Web.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Web.Tests.Controllers
{
    [TestClass]
    public class GameControllerTests
    {
        private IGameController controller;
        private IGameProcess gameProcess;
        private Context gameContext;

        [TestInitialize]
        public void Setup()
        {
            gameContext = new Context();
            gameProcess = Substitute.For<IGameProcess>();
            controller = new GameController(gameProcess, ((Func<Context>)(() => gameContext)));
        }

        [TestMethod]
        public void ShouldFailSetupIfParamsAreNotWithInDefaultRange()
        {
            //Arrange
            //Act
            Action invalidWidth = () => controller.Setup(width: 600);

            //Assert
            invalidWidth.ShouldThrow<InvalidRequestException>()
                .WithMessage("Guess width of 600 is not between 2 and 10.");
        }

        [TestMethod]
        public void SetupShouldDefaultResultLogicToPerColor()
        {
            //Arrange
            //Act
            controller.Setup(6);

            //Assert
            gameProcess.Received(1).Setup(newWidth: 6, logicType: GuessResultLogicType.PerColor);
        }

        [TestMethod]
        public void SetupShouldUsePerPegWhenStringContainsPeg()
        {
            //Arrange
            //Act
            controller.Setup(6, "143peg233245");

            //Assert
            gameProcess.Received(1).Setup(newWidth: 6, logicType: GuessResultLogicType.PerPeg);
        }

        [TestMethod]
        public void SetupShouldSetUpGameContextViaGameProcessObject()
        {
            //Arrange
            //Act
            controller.Setup(6);

            //Assert
            gameProcess.Received(1).Setup(newWidth: 6, logicType: Arg.Any<GuessResultLogicType>());
        }

        [TestMethod]
        public void SetupShouldReturnAnEmptyResultVM()
        {
            //Arrange
            gameContext.MaxAttempts = 12;

            //Act
            var results = controller.Setup(6);

            //Assert
            results.ShouldBeEquivalentTo(new GuessResultVM
            {
                Actual = new int[0],
                MaxAttempts = 12,
                Results = new FullGuessResultRowVM[0]
            });
        }

        [TestMethod]
        public void ShouldShowActualAndTotalTimeWhenGameIsOver()
        {
            //Arrange
            gameProcess.IsOver.Returns(true);
            gameContext.Actual = "rrrr".ToGuessArray();
            gameContext.Results = new List<FullGuessResultRow>();
            gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:00 am") });
            gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:05 am") });

            //Act
            var results = controller.Guess("bbbb");

            //Assert
            results.Actual.Should().BeEquivalentTo("rrrr".ToGuessArray().Select(i => (int)i));
            results.TotalTimeLapse.Should().Be(TimeSpan.FromMinutes(5));
        }

        [TestMethod]
        public void CertainFieldsShouldNotBePresentWhenGameIsNotOver()
        {
            //Arrange
            gameProcess.IsOver.Returns(false);
            gameContext.Actual = "rrrr".ToGuessArray();
            gameContext.Results = new List<FullGuessResultRow>();
            gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:00 am") });
            gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:05 am") });

            //Act
            var results = controller.Guess("bbbb");

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
            gameContext.MaxAttempts = 12;
            var results = Builder<FullGuessResultRow>
                .CreateListOfSize(2)
                .All().With(r => r.Guess = Builder<GuessColor>.CreateListOfSize(5).Build().ToArray())
                .All().With(r => r.Result = Builder<GuessResult>.CreateListOfSize(5).Build().ToArray())
                .Build()
                .ToArray();
            gameProcess.Guess(Arg.Any<string>()).Returns(results);
            gameProcess.IsAWin.Returns(true);
            gameProcess.IsOver.Returns(true);

            //Act
            var vm = controller.Guess(string.Empty);

            //Assert
            var expectedResults = Builder<FullGuessResultRowVM>
                .CreateListOfSize(2)
                .All().With(r => r.Guess = Builder<int>.CreateListOfSize(5).Build().ToArray())
                .All().With(r => r.Result = Builder<int>.CreateListOfSize(5).Build().ToArray())
                .Build()
                .ToArray();
            vm.Should().BeAssignableTo<GuessResultVM>();
            vm.Results.ShouldBeEquivalentTo(expectedResults);
            vm.IsAWin.Should().BeTrue();
            vm.IsOver.Should().BeTrue();
            vm.MaxAttempts.Should().Be(12);
        }

        [TestMethod]
        public void FinalResultShouldIncludeColorCount()
        {
            //Arrange
            gameProcess.Actual.Returns("rbry".ToGuessArray());
            gameProcess.IsOver.Returns(true);

            //Act
            var results = controller.Guess("rbry");

            //Assert
            results.ColorCount.Should().Be(3);
        }

        [TestMethod]
        public void FinalResultShouldIncludeScore()
        {
            //Arrange
            gameProcess.Actual.Returns("rbry".ToGuessArray());
            gameProcess.IsOver.Returns(true);
            gameContext.Results = new List<FullGuessResultRow>();
            gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 1:00:00 pm") });
            gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 1:00:50 pm") });

            //Act
            var results = controller.Guess("rbry");

            //Assert
            results.Score.Should().Be(96);
        }

        [TestMethod]
        public void ZeroTimeSpanShouldNotCrashScoring()
        {
            //Arrange
            gameProcess.Actual.Returns("rbry".ToGuessArray());
            gameProcess.IsOver.Returns(true);
            gameContext.Results = new List<FullGuessResultRow>();

            //Act
            var results = controller.Guess("rbry");

            //Assert
            results.Score.Should().Be(gameProcess.Actual.Length * 100);
        }
    }
}
