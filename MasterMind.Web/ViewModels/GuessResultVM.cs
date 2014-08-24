﻿using MasterMind.Core.Models;
using System;

namespace MasterMind.Web.ViewModels
{
    public class GuessResultVM
    {
        public GuessResultVM()
        {
            Actual = new GuessColor[0];
            Results = new FullGuessResultRow[0];
        }

        public FullGuessResultRow[] Results { get; set; }
        public bool IsOver { get; set; }
        public bool IsAWin { get; set; }
        public int MaxAttempts { get; set; }
        public GuessColor[] Actual { get; set; }
        public TimeSpan TotalTimeLapse { get; set; }
        public int? ColorCount { get; set; }
        public int? Score { get; set; }
    }
}