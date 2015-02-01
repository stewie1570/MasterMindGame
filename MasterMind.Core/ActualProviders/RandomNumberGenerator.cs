using System;

namespace MasterMind.Core.ActualProviders
{
    public class RandomNumberGenerator : INumberGenerator
    {
        private Random random = new Random();

        public int GetNumber(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}
