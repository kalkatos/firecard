using Kalkatos.Firecard.Core;
using System;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Performs a selection in the Match cards according to parameters.
    /// </summary>
    [Serializable]
    public class CardGetter : Getter
    {
        public CardGetterType Type;

        public CardGetter (CardGetterType type)
        {
            Type = type;
        }

        public Card[] GetCards ()
        {
            throw new NotImplementedException();
        }

        public override object Get ()
        {
            return GetCards();
        }
    }

    public enum CardGetterType
    {
        Id,
        Zone,
        FieldValue,
        Tag,
        FromTop,
        FromBottom,
        Index,
    }
}
