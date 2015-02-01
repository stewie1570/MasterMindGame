using MasterMind.Core.Models;

namespace MasterMind.Core.ActualProviders
{
    public interface IActualProvider
    {
        GuessColor[] Create(int pegCount, int repeatLimit);
    }
}
