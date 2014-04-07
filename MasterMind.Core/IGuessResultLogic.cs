using MasterMind.Core.Models;

namespace MasterMind.Core
{
    public interface IGuessResultLogic
    {
        GuessResult[] ResultFrom(Guess[] guess, Guess[] actual);
    }
}
