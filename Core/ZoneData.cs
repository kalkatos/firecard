using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Holds basic information to build a Zone.
    /// </summary>
    [Serializable]
    public class ZoneData
    {
        public string Name;
        public List<string> Tags;
    }
}