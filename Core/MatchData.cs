using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Has information to initialize a Match like what are the cards, the zones, and the rules to run the game.
    /// </summary>
    [Serializable]
    public class MatchData
    {
        public List<CardData> Cards;
        public List<ZoneData> Zones;
        public List<Rule> Rules;
        public List<(string, string)> Variables;
        public List<string> Phases;
    }
}