using System;

namespace MasterMind.Core.Models
{
    public class FullGuessResultRow
    {
        public Guess[] Guess { get; set; }
        public GuessResult[] Result { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan TimeLapse { get; set; }
    }
}
