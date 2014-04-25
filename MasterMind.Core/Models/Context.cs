using System.Collections.Generic;

namespace MasterMind.Core.Models
{
    public class Context
    {
        public int MaxAttempts { get; set; }
        public int GuessWidth { get; set; }
        public GuessColor[] Actual { get; set; }
        public List<FullGuessResultRow> Results { get; set; }
        public GuessResultLogicType ResultLogicType { get; set; }
    }
}
