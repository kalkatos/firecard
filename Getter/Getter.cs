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

    public struct Variable
    {
        public string Value;
        public static Variable USED_CARD = new Variable() { Value = "usedCard" };
        public static Variable USED_ZONE = new Variable() { Value = "usedZone" };
    }

    public struct Tag
    {
        public string Value;
        public static Tag FACE_DOWN = new Tag() { Value = "FaceDown" };
        public static Tag C (string cardTag) => new Tag() { Value = cardTag };
        public static Tag Z (string zoneTag) => new Tag() { Value = zoneTag };
    }
}
