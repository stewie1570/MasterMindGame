using System;

namespace MasterMind.Web.ViewModels
{
    public class GuessResultVM
    {
        public GuessResultVM()
        {
            Actual = new int[0];
            Results = new FullGuessResultRowVM[0];
        }

        public FullGuessResultRowVM[] Results { get; set; }
        public bool IsOver { get; set; }
        public bool IsAWin { get; set; }
        public int MaxAttempts { get; set; }
        public int[] Actual { get; set; }
        public TimeSpan TotalTimeLapse { get; set; }
        public int? ColorCount { get; set; }
        public int? Score { get; set; }
    }
}