using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using MasterMind.Core.ResultLogic;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace MasterMind.Core.Tests
{
    [TestClass]
    public class PerPegGuessResultLogicTests
    {
        private PerPegGuessResultLogic _logic;

        [TestInitialize]
        public void Setup()
        {
             _logic = new PerPegGuessResultLogic();
        }

        [TestMethod]
        public void PerfectGuessReturnsAllReds()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "rgbe".ToGuessArray(), actual: "rgbe".ToGuessArray());

            //Assert
            results.Count(r => r == GuessResult.Red).Should().Be(4);
        }

        [TestMethod]
        public void ThreeCorrectAndOneCorrectColorWrongPositionReturnsThreeRedsAndAWhite()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "rgbb".ToGuessArray(), actual: "rgbe".ToGuessArray());

            //Assert
            results.Count(r => r == GuessResult.Red).Should().Be(3);
            results.Count(r => r == GuessResult.White).Should().Be(1);
        }

        [TestMethod]
        public void ThreeCorrectAndOneWrongColorAndPostionReturnsThreeRedsAndABlank()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "rgbp".ToGuessArray(), actual: "rgbe".ToGuessArray());

            //Assert
            results.Count(r => r == GuessResult.Red).Should().Be(3);
            results.Count(r => r == GuessResult.Empty).Should().Be(1);
        }

        [TestMethod]
        public void ResultsShouldBeInOrderOfRedsWhitesEmpties()
        {
            //Arrange
            //Act
            var results = _logic.ResultFrom(guess: "pbge".ToGuessArray(), actual: "rgbe".ToGuessArray());

            //Assert
            results[0].Should().Be(GuessResult.Red);
            results[1].Should().Be(GuessResult.White);
            results[2].Should().Be(GuessResult.White);
            results[3].Should().Be(GuessResult.Empty);
        }
    }
}