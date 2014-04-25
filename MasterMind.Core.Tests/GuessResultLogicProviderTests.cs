using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterMind.Core.Models;
using FluentAssertions;

namespace MasterMind.Core.Tests
{
    [TestClass]
    public class GuessResultLogicProviderTests
    {
        [TestMethod]
        public void EmptyContextDefaultsToPerColorLogic()
        {
            new GuessResultLogicProvider(() => new Context())
                .Create().Should().BeOfType<PerColorResultLogic>();
        }

        [TestMethod]
        public void ShouldBePerColorLogic()
        {
            new GuessResultLogicProvider(() => new Context { ResultLogicType = GuessResultLogicType.PerColor })
                .Create().Should().BeOfType<PerColorResultLogic>();
        }

        [TestMethod]
        public void ShouldBePerPegLogic()
        {
            new GuessResultLogicProvider(() => new Context { ResultLogicType = GuessResultLogicType.PerPeg })
                .Create().Should().BeOfType<PerPegGuessResultLogic>();
        }
    }
}
