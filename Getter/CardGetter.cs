using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Performs a selection in the Match cards according to parameters.
    /// </summary>
    [Serializable]
    public class CardGetter : Getter
    {
        public List<CardFilter> Filters = new();

        public CardGetter () { }

        public CardGetter Id_Equals (StringGetter stringVariable)
        {
            if (Filters == null)
                Filters = new();
            Filters.Add(new CardFilter_Id(stringVariable));
            return this;
        }

        public CardGetter Zone (ZoneGetter zoneGetter)
        {
            if (Filters == null)
                Filters = new();
            Filters.Add(new CardFilter_Zone(zoneGetter));
            return this;
        }

        public CardGetter Tag_Equals (StringGetter tag)
        {
            // TODO 
            return this;
        }

        public CardGetter Tag_NotEquals (StringGetter tag)
        {
            // TODO 
            return this;
        }

        public CardGetter Tag_Any (params StringGetter[] tags)
        {
            // TODO 
            return this;
        }

        public CardGetter Tag_NotAny (params StringGetter[] tags)
        {
            // TODO 
            return this;
        }

        public CardGetter Tag_All (params StringGetter[] tags)
        {
            // TODO 
            return this;
        }

        public CardGetter Field (FieldGetter fieldGetter)
        {
            // TODO 
            return this;
        }

        public Card[] GetCards ()
        {
            List<Card> cards = new List<Card>(Match.GetState().Cards);
            Filter(cards);
            return cards.ToArray();
        }

        public override object Get ()
        {
            return GetCards();
        }

        private void Filter (List<Card> cards)
        {
            cards = cards.Where((c) => Filters.TrueForAll((f) => f.IsMatch(c))).ToList();
        }
    }

    [Serializable]
    public class CardFilter
    {
        public CardGetterType Type;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StringGetter StringParameter1;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StringGetter StringParameter2;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ZoneGetter ZoneParameter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NumberGetter NumberParameter;

        public virtual bool IsMatch (Card card)
        {
            switch (Type)
            {
                case CardGetterType.Id:
                    return MatchById(card);
                case CardGetterType.Zone:
                    return MatchByZone(card);
                case CardGetterType.NumericFieldValue:
                    return MatchByNumericField(card);
                case CardGetterType.StringFieldValue:
                    return MatchByStringField(card);
                case CardGetterType.Tag:
                    break;
                case CardGetterType.FromTop:
                    break;
                case CardGetterType.FromBottom:
                    break;
                case CardGetterType.Index:
                    break;
                default:
                    throw new NotImplementedException($"Card getter type not implemented: {Type}.");
            }
            return false;
        }

        protected bool MatchById (Card card)
        {
            string parValue = StringParameter1.GetString();
            return card.id == parValue || card.id == Match.GetStringVariable(parValue);
        }

        protected bool MatchByZone (Card card)
        {
            Zone[] zones = ZoneParameter.GetZones();
            if (zones == null || zones.Length == 0)
                return false;
            return card.currentZone == zones[0];
        }

        protected bool MatchByNumericField (Card card)
        {
            string fieldName = StringParameter1.GetString();
            return card.GetNumericFieldValue(fieldName) == NumberParameter.GetNumber();
        }

        protected bool MatchByStringField (Card card)
        {
            string fieldName = StringParameter1.GetString();
            return card.GetStringFieldValue(fieldName) == StringParameter2.GetString();
        }
    }

    public class CardFilter_Id : CardFilter
    {
        public CardFilter_Id (StringGetter stringVariable)
        {
            Type = CardGetterType.Id;
            StringParameter1 = stringVariable;
        }

        public override bool IsMatch (Card card)
        {
            return MatchById(card);
        }
    }

    public class CardFilter_Zone : CardFilter
    {
        public CardFilter_Zone (ZoneGetter zoneGetter)
        {
            Type = CardGetterType.Zone;
            ZoneParameter = zoneGetter;
        }

        public override bool IsMatch (Card card)
        {
            return MatchByZone(card);
        }
    }

    public enum CardGetterType
    {
        Id,
        Zone,
        NumericFieldValue,
        StringFieldValue,
        Tag,
        FromTop,
        FromBottom,
        Index,
    }
}
