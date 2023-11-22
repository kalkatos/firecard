using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// The rules define how the game will run. At each stage of the running Match, it will run through the relevant rules and, if their condition is met, their effects will be executed to make changes in the MatchState.
    /// </summary>
    [Serializable]
    public class Rule
    {
        public string Name;
        public Trigger Trigger;
        public Condition Condition;
        public List<Effect> TrueEffects;
        public List<Effect> FalseEffects;

        internal string id;
    }
}