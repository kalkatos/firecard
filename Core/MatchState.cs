using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Contains instant information about a running game like what cards are in which zones, what info each card field has, what are the players' actions, what are the values in variables, etc.
    /// </summary>
    [Serializable]
    public class MatchState
    {
        public List<Card> Cards;
        public List<Zone> Zones;
    }
}