using MasterMind.Core.Models;
using System;

namespace MasterMind.Core
{
    public interface IGameProcess
    {
        GuessColor[] Actual { get; }
        FullGuessResultRow[] Guess(string guessString);
        bool IsAWin { get; }
        bool IsOver { get; }
        void Setup(int newWidth, GuessResultLogicType logicType);
    }
}
