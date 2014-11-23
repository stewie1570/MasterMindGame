using System;

namespace MasterMind.Web.ViewModels
{
    public class FullGuessResultRowVM
    {
        public int[] Guess { get; set; }
        public int[] Result { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan TimeLapse { get; set; }
    }
}