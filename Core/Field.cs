using System;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Holds information inside Card.
    /// </summary>
    [Serializable]
    public struct Field
    {
        public string Name;
        public float NumericValue;
        public string StringValue;

        public Field (Field other)
        {
            Name = other.Name;
            NumericValue = other.NumericValue;
            StringValue = other.StringValue;
        }

        public Field (string name, float numericValue, string stringValue)
        {
            Name = name;
            NumericValue = numericValue;
            StringValue = stringValue;
        }
    }
}