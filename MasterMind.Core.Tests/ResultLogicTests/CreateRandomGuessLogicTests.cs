using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace MasterMind.Core.ResultLogic.Tests
{
    [TestClass]
    public class CreateRandomGuessLogicTests
    {
        [TestMethod]
        public void RandomGuessArrayShouldBeCorrectLength()
        {
            //Arrange
            //Act
            //Assert
            CreateRandomActualLogic.Create(4).Length.Should().Be(4);
        }

        /// <summary>
        /// NOTE: This test may fail (1 chance in 6^(count of guess enum values)).
        /// </summary>
        [TestMethod]
        public void RandomGuessArrayShouldBeRandom()
        {
            //Arrange
            //Act
            var guess = CreateRandomActualLogic.Create(4);
            
            //Assert
            var first = guess.First();
            guess.Count(g => g == first).Should().BeLessThan(guess.Length);
        }
    }
}
