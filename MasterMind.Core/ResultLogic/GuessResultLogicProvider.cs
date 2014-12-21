using MasterMind.Core.Models;
using System;

namespace MasterMind.Core.ResultLogic
{
    public class GuessResultLogicProvider : IGuessResultLogicProvider
    {
        private Func<GameContext> _contextProvider;

        public GuessResultLogicProvider(Func<GameContext> contextProvider)
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
