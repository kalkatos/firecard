using Kalkatos.Firecard.Utility;
using System;

namespace Kalkatos.Firecard.Core
{
    [Serializable]
    public class Card
    {
        internal int index;

        public int Index => index;

        public float GetNumericFieldValue (string fieldName)
        {
            throw new NotImplementedException();
        }

        public float GetNumericFieldValue (StringGetter fieldName)
        {
            return GetNumericFieldValue(fieldName.GetString());
        }

        public string GetStringFieldValue (string fieldName)
        {
            throw new NotImplementedException();
        }

        public string GetStringFieldValue (StringGetter fieldName)
        {
            return GetStringFieldValue(fieldName.GetString());
        }
    }
}