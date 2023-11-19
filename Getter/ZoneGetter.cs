using Kalkatos.Firecard.Core;
using System;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Performs a selection in the Match zones according to parameters.
    /// </summary>
    [Serializable]
    public class ZoneGetter : Getter
    {
        public Zone[] GetZones ()
        {
            throw new NotImplementedException();
        }

        public override object Get ()
        {
            return GetZones();
        }
    }
}
