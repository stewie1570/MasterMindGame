using MasterMind.Core.Models;
using System.Collections.Generic;

namespace MasterMind.Core
{
    public class ContextProvider
    {
        private CurrentUser currentUser;
        private readonly int maxContextCount = 5;

        public ContextProvider(CurrentUser currentUser)
        {
            this.currentUser = currentUser;
        }

        public void Add(GameContext context)
        {
            if (currentUser.Contexts == null)
                currentUser.Contexts = new List<GameContext>();
            if (currentUser.Contexts.Count < maxContextCount)
                currentUser.Contexts.Add(context);
        }
    }
}
