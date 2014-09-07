
namespace MasterMind.Core.Models.Extensions
{
    public static class GuessResultLogicTypeExtensions
    {
        public static GuessResultLogicType ToGuessResultLogicType(this string resultLogic)
        {
            return resultLogic.ToLower().Contains("peg")
                ? GuessResultLogicType.PerPeg
                : GuessResultLogicType.PerColor;
        }
    }
}
