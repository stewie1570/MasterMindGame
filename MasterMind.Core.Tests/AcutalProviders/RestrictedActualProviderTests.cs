using FluentAssertions;
using MasterMind.Core.ActualProviders;
using MasterMind.Core.Extensions;
using MasterMind.Core.Models;
using MasterMind.Core.Models.Extensions;
using MasterMind.Core.NumberGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;

namespace MasterMind.Core.Tests.AcutalProviders
{
    [TestClass]
    public class RestrictedActualProviderTests
    {
        private IActualProvider provider;
        private INumberGenerator numberGenerator;

        [TestInitialize]
        public void Setup()
        {
            numberGenerator = Substitute.For<INumberGenerator>();
            provider = new RestrictedActualProvider(numberGenerator);
        }

        [TestMethod]
        public void ShouldReturnNumberGeneratorSelectedColors()
        {
            //Arrange
            int possiblesCount = Enum.GetValues(typeof(GuessColor)).Cast<GuessColor>().Count();
            int currentNumber = 1;
            numberGenerator
                .GetNumber(minValue: 1, maxValue: possiblesCount)
                .Returns(ci => currentNumber++);

            //Act
            //Assert
            provider.Create(pegCount: 5, repeatLimit: 5).Should().BeEquivalentTo("rbgyp".ToGuessArray());
        }

        [TestMethod]
        public void ShouldNotRepeatPegsMoreThanLimitAllows()
        {
            //Arrange
            numberGenerator.GetNumber(minValue: Arg.Any<int>(), maxValue: Arg.Any<int>()).Returns(1);

            //Act
            //Assert
            provider.Create(pegCount: 5, repeatLimit: 5).Should().BeEquivalentTo("rrrrr".ToGuessArray());
            provider.Create(pegCount: 5, repeatLimit: 4).Should().BeEquivalentTo("rrrrb".ToGuessArray());
            provider.Create(pegCount: 5, repeatLimit: 3).Should().BeEquivalentTo("rrrbb".ToGuessArray());
            provider.Create(pegCount: 5, repeatLimit: 2).Should().BeEquivalentTo("rrbbg".ToGuessArray());
            provider.Create(pegCount: 5, repeatLimit: 1).Should().BeEquivalentTo("rbgyp".ToGuessArray());
        }

        [TestMethod]
        public void MakeSurePegCountWithRepeatLimitIsPossibleConsideringNumberOfPossibleColors()
        {
            //Arrange
            int possiblesCount = Enum.GetValues(typeof(GuessColor)).Cast<GuessColor>().Count() - 1;
            //possibleCount - 1 because empty is included in the enum but can't be in the actual.

            //Act
            //Assert
            ((Action)(() => provider.Create(pegCount: 50, repeatLimit: 1)))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("50 peg(s) with only 1 repeating peg(s) requires 50 colors. Only {0} are available."
                    .FormatWith(possiblesCount));

            ((Action)(() => provider.Create(pegCount: 40, repeatLimit: 2)))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("40 peg(s) with only 2 repeating peg(s) requires 20 colors. Only {0} are available."
                    .FormatWith(possiblesCount));

            ((Action)(() => provider.Create(pegCount: 36, repeatLimit: 6)))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("36 peg(s) with only 6 repeating peg(s) requires 6 colors. Only {0} are available."
                    .FormatWith(possiblesCount));
        }

        [TestMethod]
        public void PegCountAndRepeatLimitMustBePositive()
        {
            ((Action)(() => provider.Create(pegCount: -1, repeatLimit: 1)))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("Peg count and repeat limit must be positive.");

            ((Action)(() => provider.Create(pegCount: 50, repeatLimit: -1)))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("Peg count and repeat limit must be positive.");
        }
    }
}
