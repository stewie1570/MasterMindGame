using System;

namespace MasterMind.Core.NumberGenerators
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
