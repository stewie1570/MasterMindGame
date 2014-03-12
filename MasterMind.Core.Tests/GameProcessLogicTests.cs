using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FizzWare.NBuilder;

namespace MasterMind.Core.Tests
{
    [TestClass]
    public class GameProcessLogicTests
    {
        private GameProcess _game;
        private Context _context;

        [TestInitialize]
        public void Setup()
        {
            _context = new Context
            {
                GuessWidth = 4,
                MaxAttempts = 10,
            };
            _game = new GameProcess(() => _context,
                length => string.Empty.PadLeft(length, 'b').ToGuessArray());
        }

        [TestMethod]
        public void ConstructorShouldNotResetContext()
        {
            //Arrange
            var context = new Context
            {
                Actual = "rrrr".ToGuessArray(),
                Results = Builder<FullGuessResultRow>.CreateListOfSize(2).Build().ToList(),
                GuessWidth = 4
            };

            //Act
            new GameProcess(() => context, length => "".PadLeft(length, 'b').ToGuessArray());

            //Assert
            context.Results.Count.Should().Be(2);
            context.Actual.Should().BeEquivalentTo("rrrr".ToGuessArray());
        }

        [TestMethod]
        public void SetupShouldResetGameContext()
        {
            //Arrange
            _game.Guess("rrrr");

            //Act
            _game.Setup(newWidth: 5, newMaxAttempts: 12);
            _game.Guess("rrrrr");

            //Assert
            _context.MaxAttempts.Should().Be(12);
            _context.GuessWidth.Should().Be(5);
            _context.Results.Count.Should().Be(1);
        }

        [TestMethod]
        public void ConstructorShouldInitializeContext()
        {
            //Arrange
            var context = new Context
            {
                GuessWidth = 4,
                MaxAttempts = 10,
            };

            //Act
            _game = new GameProcess(() => context, length => "bbbb".ToGuessArray());
            _game.Guess("rrrr");

            //Assert
            context.MaxAttempts.Should().Be(10);
            context.GuessWidth.Should().Be(4);
            context.Actual.Should().BeEquivalentTo("bbbb".ToGuessArray());
            context.Results.Count.Should().Be(1);
        }

        [TestMethod]
        public void ShouldResetActualWhenActualIsWrongLength()
        {
            //Arrange
            //Act
            var game = new GameProcess(() => new Context
            {
                GuessWidth = 5,
                MaxAttempts = 10,
                Actual = "yyyy".ToGuessArray()
            }, length => "rrrrr".ToGuessArray());

            //Assert
            game.Actual.Should().BeEquivalentTo("rrrrr".ToGuessArray());
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
