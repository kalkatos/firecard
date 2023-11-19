using System;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Abstract class for something that gets information.
    /// </summary>
    [Serializable]
    public abstract class Getter
    {
        public abstract object Get ();
    }
}
