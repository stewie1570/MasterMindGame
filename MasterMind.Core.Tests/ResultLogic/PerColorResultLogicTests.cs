using FluentAssertions;
using MasterMind.Core.Models.Extensions;
using MasterMind.Core.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterMind.Core.ResultLogic.Tests
{
    [TestClass]
    public class PerColorResultLogicTests
    {
        private IGuessResultLogic _logic;

        [TestInitialize]
        public void Setup()
        {
            _logic = new PerColorResultLogic();
        }

        [TestMethod]
        public void TwoReds()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "eebb".ToGuessArray(), actual: "rrbb".ToGuessArray());

            //Assert
            results.AsResultString().Should().Be("rree");
        }

        [TestMethod]
        public void TwoRedsTwoEmpties()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "bbbb".ToGuessArray(), actual: "rrbb".ToGuessArray());

            //Assert
            results.AsResultString().Should().Be("rree");
        }

        [TestMethod]
        public void TwoRedsOneWhiteOneEmpty()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "grbb".ToGuessArray(), actual: "rybb".ToGuessArray());

            //Assert
            results.AsResultString().Should().Be("rrwe");
        }

        [TestMethod]
        public void TwoRedsTwoEmpties_OverGuessingAColor()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "ebbb".ToGuessArray(), actual: "rybb".ToGuessArray());

            //Assert
            results.AsResultString().Should().Be("rree");
        }

        [TestMethod]
        public void OneRed_UnderGuessingAColor()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "eeeb".ToGuessArray(), actual: "rybb".ToGuessArray());

            //Assert
            results.AsResultString().Should().Be("reee");
        }
    }
}
