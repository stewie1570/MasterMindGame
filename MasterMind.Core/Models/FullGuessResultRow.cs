using System;

namespace MasterMind.Core.Models
{
    public class FullGuessResultRow
    {
        public GuessColor[] Guess { get; set; }
        public GuessResult[] Result { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan TimeLapse { get; set; }
    }
}
