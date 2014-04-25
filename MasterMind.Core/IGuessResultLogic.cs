using MasterMind.Core.Models;

namespace MasterMind.Core
{
    public interface IGuessResultLogic
    {
        GuessResult[] ResultFrom(GuessColor[] guess, GuessColor[] actual);
    }
}
