using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Utility
{
    [Serializable]
    public class FieldGetter : Getter
    {
        [JsonProperty]
        internal string fieldName;
        [JsonProperty]
        internal StringGetter fieldNameGetter;
        [JsonProperty]
        internal CardGetter cardGetter;

        private string FieldName => (string.IsNullOrEmpty(fieldName) ? fieldNameGetter?.GetString() : fieldName);

        public FieldGetter () { }

        public FieldGetter (string fieldName, CardGetter cardGetter)
        {
            this.fieldName = fieldName;
            this.cardGetter = cardGetter;
        }

        public override object Get ()
        {
            if (string.IsNullOrEmpty(FieldName))
                return null;
            List<Card> cards = cardGetter.GetCards();
            if (cards.Count == 0)
                return null;
            return cards[0].GetFieldValue(FieldName);
        }

        public string GetStringValue ()
        {
            List<Card> cards = cardGetter.GetCards();
            if (cards.Count == 0)
                return null;
            return cards[0].GetTextFieldValue(FieldName);
        }

        public float GetNumberValue ()
        {
            List<Card> cards = cardGetter.GetCards();
            if (cards.Count == 0)
                return float.NaN;
            return cards[0].GetNumericFieldValue(FieldName);
        }
    }
}
