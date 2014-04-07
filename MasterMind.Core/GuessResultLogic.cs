using MasterMind.Core.Models;
using System.Linq;

namespace MasterMind.Core
{
    public class GuessResultLogic : IGuessResultLogic
    {
        public GuessResult[] ResultFrom(Guess[] guess, Guess[] actual)
        {
            var result = new GuessResult[guess.Length];

            for (var i = 0; i < guess.Length; i++)
                result[i] = EvalResultFrom(guess, actual, i);

            return result.OrderBy(r => r).ToArray();
        }

        #region Helpers

        private static GuessResult EvalResultFrom(Guess[] guess, Guess[] actual, int i)
        {
            return (guess[i] == actual[i])
                ?
                    GuessResult.Red
                : actual.Contains(guess[i])
                    ?
                        GuessResult.White
                    :
                        GuessResult.Empty;
        }

        #endregion
    }
}
