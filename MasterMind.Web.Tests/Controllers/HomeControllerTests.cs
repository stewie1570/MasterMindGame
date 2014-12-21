﻿using FizzWare.NBuilder;
using FluentAssertions;
using MasterMind.Core;
using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using MasterMind.Web.Controllers;
using MasterMind.Web.Exceptions;
using MasterMind.Web.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MasterMind.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private IGameProcess _gameProcess;
        private GameContext _gameContext;

        [TestInitialize]
        public void Setup()
        {
            _gameContext = new GameContext();
            _gameProcess = Substitute.For<IGameProcess>();
            _controller = new HomeController(_gameProcess, () => _gameContext);
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
            _gameProcess.Received(1).Setup(newWidth: 6, logicType: GuessResultLogicType.PerColor);
        }

        [TestMethod]
        public void SetupShouldUsePerPegWhenStringContainsPeg()
        {
            //Arrange
            //Act
            _controller.Setup(6, "143peg233245");

            //Assert
            _gameProcess.Received(1).Setup(newWidth: 6, logicType: GuessResultLogicType.PerPeg);
        }

        [TestMethod]
        public void SetupShouldSetUpGameContextViaGameProcessObject()
        {
            //Arrange
            //Act
            _controller.Setup(6);

            //Assert
            _gameProcess.Received(1).Setup(newWidth: 6, logicType: Arg.Any<GuessResultLogicType>());
        }

        [TestMethod]
        public void SetupShouldReturnAnEmptyResultVM()
        {
            //Arrange
            _gameContext.MaxAttempts = 12;

            //Act
            GuessResultVM results = (_controller.Setup(6) as JsonResult).Data as GuessResultVM;

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
            _gameProcess.IsOver.Returns(true);
            _gameContext.Actual = "rrrr".ToGuessArray();
            _gameContext.Results = new List<FullGuessResultRow>();
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:00 am") });
            _gameContext.Results.Add(new FullGuessResultRow { TimeStamp = DateTime.Parse("1/1/2000 12:05 am") });

            //Act
            var results = (_controller.Guess("bbbb") as JsonResult).Data as GuessResultVM;

            //Assert
            results.Actual.Should().BeEquivalentTo("rrrr".ToGuessArray().Select(i => (int)i));
            results.TotalTimeLapse.Should().Be(TimeSpan.FromMinutes(5));
        }

        [TestMethod]
        public void CertainFieldsShouldNotBePresentWhenGameIsNotOver()
        {
            //Arrange
            _gameProcess.IsOver.Returns(false);
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
            var results = Builder<FullGuessResultRow>
                .CreateListOfSize(2)
                .All().With(r => r.Guess = Builder<GuessColor>.CreateListOfSize(5).Build().ToArray())
                .All().With(r => r.Result = Builder<GuessResult>.CreateListOfSize(5).Build().ToArray())
                .Build()
                .ToArray();
            _gameProcess.Guess(Arg.Any<string>()).Returns(results);
            _gameProcess.IsAWin.Returns(true);
            _gameProcess.IsOver.Returns(true);

            //Act
            var vm = (_controller.Guess(string.Empty) as JsonResult).Data as GuessResultVM;

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
            _gameProcess.Actual.Returns("rbry".ToGuessArray());
            _gameProcess.IsOver.Returns(true);

            //Act
            var results = (_controller.Guess("rbry") as JsonResult).Data as GuessResultVM;

            //Assert
            results.ColorCount.Should().Be(3);
        }

        [TestMethod]
        public void FinalResultShouldIncludeScore()
        {
            //Arrange
            _gameProcess.Actual.Returns("rbry".ToGuessArray());
            _gameProcess.IsOver.Returns(true);
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
            _gameProcess.Actual.Returns("rbry".ToGuessArray());
            _gameProcess.IsOver.Returns(true);
            _gameContext.Results = new List<FullGuessResultRow>();

            //Act
            var results = (_controller.Guess("rbry") as JsonResult).Data as GuessResultVM;

            //Assert
            results.Score.Should().Be(_gameProcess.Actual.Length * 100);
        }
    }
}
