using MasterMind.Web.ViewModels;

namespace MasterMind.Web.Interfaces
{
    public interface IGameController
    {
        GuessResultVM Guess(string guess);
        GuessResultVM Setup(int width, string resultLogic = "percolor");
    }
}
