using MasterMind.Core.Models.Extensions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;

namespace MasterMind.Core.Tests
{
    [TestClass]
    public class GameProcessLogicTests
    {
        private GameProcess _game;

        [TestInitialize]
        public void Setup()
        {
            _game = new GameProcess(10, 4, length => "bbbb".ToGuessArray());
        }

        [TestMethod]
        public void ShouldCreateActualInConstructor()
        {
            //Arrange
            //Act
            //Assert
            _game.Actual.ToGuessString().Should().Be("bbbb");
        }

        [TestMethod]
        public void GuessResultCountShouldBeEqualToGuessAttemptCount()
        {
            //Arrange
            //Act
            _game.Guess("yyyy");
            var results = _game.Guess("rrrr");

            //Assert
            results.Length.Should().Be(2);
        }

        [TestMethod]
        public void GameIsOverAndLostWhenGuessCountIsReached()
        {
            //Arrange
            //Act
            Enumerable.Range(1, 10).ToList().ForEach(i => _game.Guess("yyyy"));

            //Assert
            _game.IsOver.Should().BeTrue();
            _game.IsAWin.Should().BeFalse();
        }

        [TestMethod]
        public void GameIsOverAndWonWhenGuessIsCorrect()
        {
            //Arrange
            //Act
            _game.Guess("bbbb");

            //Assert
            _game.IsOver.Should().BeTrue();
            _game.IsAWin.Should().BeTrue();
        }

        [TestMethod]
        public void NoResultsAreAddedAfterGameOver()
        {
            //Arrange
            //Act
            Enumerable.Range(1, 20).ToList().ForEach(i => _game.Guess("yyyy"));
            var results = _game.Guess("yyyy");

            //Assert
            results.Length.Should().Be(10);
        }

        [TestMethod]
        public void EmptyGuessShouldThrow()
        {
            //Arrange
            //Act
            Action action = () => _game.Guess("");

            //Assert
            action.ShouldThrow<InvalidGuessException>();
        }
    }
}
