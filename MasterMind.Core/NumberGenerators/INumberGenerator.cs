
namespace MasterMind.Core.NumberGenerators
{
    public interface INumberGenerator
    {
        int GetNumber(int minValue, int maxValue);
    }
}
