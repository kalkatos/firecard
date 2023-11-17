using Kalkatos.Firecard.Core;
using System;

namespace Kalkatos.Firecard.Utility
{
    [Serializable]
    public abstract class Getter
    {
        public abstract object Get ();
    }

    [Serializable]
    public class CardGetter : Getter
    {
        public Card[] GetCards ()
        {
            throw new NotImplementedException();
        }

        public override object Get ()
        {
            return GetCards();
        }
    }

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
