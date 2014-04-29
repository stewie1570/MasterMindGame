using MasterMind.Core.Models;

namespace MasterMind.Core.ResultLogic
{
    public interface IGuessResultLogic
    {
        GuessResult[] ResultFrom(GuessColor[] guess, GuessColor[] actual);
    }
}
