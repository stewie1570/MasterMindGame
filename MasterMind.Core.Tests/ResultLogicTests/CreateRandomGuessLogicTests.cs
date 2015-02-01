using FluentAssertions;
using MasterMind.Core.ActualProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MasterMind.Core.ResultLogic.ActualProviders.Tests
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
            RandomActualProvider.Create(4).Length.Should().Be(4);
        }

        /// <summary>
        /// NOTE: This test may fail (1 chance in 6^(count of guess enum values)).
        /// </summary>
        [TestMethod]
        public void RandomGuessArrayShouldBeRandom()
        {
            //Arrange
            //Act
            var guess = RandomActualProvider.Create(4);
            
            //Assert
            var first = guess.First();
            guess.Count(g => g == first).Should().BeLessThan(guess.Length);
        }
    }
}
