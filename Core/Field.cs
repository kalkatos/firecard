using Kalkatos.Firecard.Utility;
using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Holds information inside Card.
    /// </summary>
    [Serializable]
    public struct Field
    {
        public string Name;
        public float? Number;
        public string Text;

        public Field (Field other)
        {
            Name = other.Name;
            Number = other.Number.HasValue ? other.Number.Value : null;
            Text = other.Text;
        }

        public Field (string name, float numericValue, string stringValue)
        {
            Name = name;
            Number = numericValue;
            Text = stringValue;
        }

        public bool IsNumber ()
        {
            return Number.HasValue && Text == null;
        }

        public bool IsText ()
        {
            return Text != null && !Number.HasValue;
        }

        internal static string GetText (string fieldName, CardGetter cardGetter)
        {
            List<Card> cards = cardGetter.GetCards();
            if (cards.Count > 0)
                return cards[0].GetTextFieldValue(fieldName);
            return null;
        }

        internal static float GetNumber (string fieldName, CardGetter cardGetter)
        {
            List<Card> cards = cardGetter.GetCards();
            if (cards.Count > 0)
                return cards[0].GetNumericFieldValue(fieldName);
            return float.NaN;
        }

        public static FieldGetter Getter (string fieldName, CardGetter cardGetter)
        {
            return new FieldGetter(fieldName, cardGetter);
        }
    }
}