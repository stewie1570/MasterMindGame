using MasterMind.Core.Models;

namespace MasterMind.Web.ViewModels
{
    public class GuessResultVM
    {
        public FullGuessResultRow[] Results { get; set; }
        public bool IsOver { get; set; }
        public bool IsAWin { get; set; }
        public int MaxAttempts { get; set; }
    }
}