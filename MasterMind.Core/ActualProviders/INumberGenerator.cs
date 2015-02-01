
namespace MasterMind.Core.ActualProviders
{
    public interface INumberGenerator
    {
        int GetNumber(int minValue, int maxValue);
    }
}
