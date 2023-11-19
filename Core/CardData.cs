using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Holds static information for the initial setup of cards.
    /// </summary>
    [Serializable]
    public class CardData
    {
        public string Name;
        public string Description;
        public List<string> Tags;
        public List<Field> Fields;
    }
}