using MasterMind.Core;
using MasterMind.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GameProcess game = new GameProcess(() => new Context
                    {
                        GuessWidth = int.Parse(Input("Guess Width")),
                        MaxAttempts = int.Parse(Input("Max Attempts"))
                    },
                    actualProvider: width => CreateGuessLogic.Create(width));

                while (!game.IsOver)
                {
                    try
                    {
                        string possibleColors = string.Join(", ", Enum.GetNames(typeof(GuessColor)));
                        Console.WriteLine("Possible Colors: " + possibleColors);
                        var results = game.Guess(Input("Guess one letter per color"));
                        Console.Clear();
                        OuputResults(results);
                    }
                    catch (Exception ex) { ShowException(ex.Message); }
                }

                ShowEndOfGame(game);
            }
            catch (Exception ex) { ShowException(ex.ToString()); }

            Console.ReadKey();
        }

        #region Helpers

        private static void ShowEndOfGame(GameProcess game)
        {
            Console.WriteLine();
            OutputGuess(game.Actual.ToList());
            Console.WriteLine();
            Console.WriteLine();

            string message = game.IsAWin ? "You Won!!!" : "You Lost";
            OutputInColor(() => Console.Write(message), game.IsAWin ? ConsoleColor.Green : ConsoleColor.Red);
        }

        private static void ShowException(string message)
        {
            Console.WriteLine();
            OutputInColor(() => Console.Write(message), ConsoleColor.Red);
            Console.WriteLine();
        }

        private static void OuputResults(FullGuessResultRow[] resultRow)
        {
            for(int i = 0;i < resultRow.Length;i++)
            {
                var row = resultRow[i];

                Console.Write(((i + 1) + ": ").PadRight(6, ' '));

                OutputGuess(row.Guess.ToList());

                Console.Write(" ");

                row.Result.ToList().ForEach(result =>
                {
                    if (result == GuessResult.Red)
                        OutputInColor(() => Console.Write("*"), ConsoleColor.Red);
                    else if (result == GuessResult.White)
                        OutputInColor(() => Console.Write("*"), ConsoleColor.White);
                    else
                        OutputInColor(() => Console.Write("."), ConsoleColor.DarkGray);
                });
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static void OutputGuess(List<GuessColor> guesses)
        {
            guesses.ForEach(guess =>
            {
                switch (guess)
                {
                    case GuessColor.Blue:
                        OutputInColor(() => Console.Write("b"), ConsoleColor.Blue);
                        break;
                    case GuessColor.Empty:
                        OutputInColor(() => Console.Write("e"), ConsoleColor.DarkGray);
                        break;
                    case GuessColor.Green:
                        OutputInColor(() => Console.Write("g"), ConsoleColor.Green);
                        break;
                    case GuessColor.Purple:
                        OutputInColor(() => Console.Write("p"), ConsoleColor.Magenta);
                        break;
                    case GuessColor.Red:
                        OutputInColor(() => Console.Write("r"), ConsoleColor.Red);
                        break;
                    case GuessColor.Yellow:
                        OutputInColor(() => Console.Write("y"), ConsoleColor.Yellow);
                        break;
                }
            });
        }

        private static void OutputInColor(Action output, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            output();
            Console.ForegroundColor = originalColor;
        }

        private static string Input(string description)
        {
            Console.Write(description + ": ");
            return Console.ReadLine();
        }

        #endregion
    }
}
