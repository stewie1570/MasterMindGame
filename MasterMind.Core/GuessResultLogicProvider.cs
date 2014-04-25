using MasterMind.Core.Models;
using System;

namespace MasterMind.Core
{
    public class GuessResultLogicProvider : IGuessResultLogicProvider
    {
        private Func<Context> _contextProvider;

        public GuessResultLogicProvider(Func<Context> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public IGuessResultLogic Create()
        {
            return _contextProvider().ResultLogicType == GuessResultLogicType.PerPeg
                ? (IGuessResultLogic) new PerPegGuessResultLogic()
                : (IGuessResultLogic) new PerColorResultLogic();
        }
    }
}
