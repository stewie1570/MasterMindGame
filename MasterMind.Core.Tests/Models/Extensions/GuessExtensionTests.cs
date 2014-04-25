using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace MasterMind.Core.Tests.Models.Extensions
{
    [TestClass]
    public class GuessExtensionTests
    {
        [TestMethod]
        public void ToStringShouldConvertGuessListToString()
        {
            //Arrange
            var guess = new GuessColor[] { GuessColor.Blue, GuessColor.Empty, GuessColor.Green, GuessColor.Red };

            //Act
            //Assert
            guess.ToGuessString().Should().Be("begr");
        }

        [TestMethod]
        public void ToGuessArrayShouldConvertStringToGuessArray()
        {
            //Arrange
            var guess = new GuessColor[] { GuessColor.Blue, GuessColor.Empty, GuessColor.Green, GuessColor.Red };

            //Act
            //Assert
            "begr".ToGuessArray().ShouldBeEquivalentTo(guess);
        }

        [TestMethod]
        public void ToGuessArrayShouldBeCaseInsensitive()
        {
            //Arrange
            var guess = new GuessColor[] { GuessColor.Blue, GuessColor.Empty, GuessColor.Green, GuessColor.Red };

            //Act
            //Assert
            "BeGr".ToGuessArray().ShouldBeEquivalentTo(guess);
        }

        [TestMethod]
        public void NotExistentColorsShouldThrowInvalidGuessException()
        {
            //Arrange
            var guess = new GuessColor[] { GuessColor.Blue, GuessColor.Empty, GuessColor.Green, GuessColor.Red };

            //Act
            Action action = () => "bmno".ToGuessArray();

            //Assert
            action.ShouldThrow<InvalidGuessException>()
                .WithMessage("The following colors (m, n, o) do not exist in this game.");
        }

        [TestMethod]
        public void WrongLengthGuessShouldThrowInvalidGuessException()
        {
            //Arrange
            var guess = new GuessColor[] { GuessColor.Blue, GuessColor.Empty, GuessColor.Green, GuessColor.Red };

            //Act
            Action action = () => "bBbBbb".ToGuessArray(expectedLength: 4);

            //Assert
            action.ShouldThrow<InvalidGuessException>()
                .Where(e => e.Message == "The guess \"bBbBbb\" has a length of 6. Length should be 4.");
        }
    }
}
