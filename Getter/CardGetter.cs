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
        [JsonProperty]
        internal List<Filter<Card>> Filters = new();

        public CardGetter () { }

        public static CardGetter New => new CardGetter();

        public CardGetter Id (string id)
        {
            return Id(id, Operation.Equals);
        }

        public CardGetter Id (string id, Operation operation)
        {
            Filters.Add(new CardFilter_Id(id, operation));
            return this;
        }

        public CardGetter Zone (string tag)
        {
            Filters.Add(new CardFilter_Zone(tag));
            return this;
        }

        public CardGetter Zone (ZoneGetter zoneGetter, Operation operation)
        {
            Filters.Add(new CardFilter_Zone(zoneGetter, operation));
            return this;
        }

        public CardGetter Tag (string tag)
        {
            return Tag(new ZoneGetter().Tag(tag), Operation.Equals);
        }

        public CardGetter Tag (ZoneGetter zoneGetter, Operation operation)
        {
            Filters.Add(new CardFilter_Zone(zoneGetter, operation));
            return this;
        }

        public CardGetter Tag (StringGetter tag)
        {
            // TODO 
            return this;
        }

        public CardGetter Field (FieldGetter fieldGetter)
        {
            // TODO 
            return this;
        }

        public CardGetter Amount (int value)
        {
            Filters.Add(new CardFilter_Amount(value));
            return this;
        }

        public List<Card> GetCards ()
        {
            List<Card> cards = new List<Card>(Match.GetState().Cards);
            Filter(cards);
            return cards;
        }

        public override object Get ()
        {
            return GetCards();
        }

        private void Filter (List<Card> cards)
        {
            if (cards.Count == 0)
                return;
            cards = cards.Where((c) => Filters.TrueForAll((f) => f.IsMatch(c))).ToList();
        }
    }

    [Serializable]
    internal class CardFilter_Id : Filter<Card>
    {
        [JsonProperty]
        internal string plainString;

        internal CardFilter_Id () { }

        internal CardFilter_Id (string id, Operation operation)
        {
            Operation = operation;
            plainString = id;
        }

        internal override bool IsMatch (Card card)
        {
            return false;
        }
    }

    [Serializable]
    internal class CardFilter_Zone : Filter<Card>
    {
        [JsonProperty]
        internal string simpleTag;
        [JsonProperty]
        internal ZoneGetter zoneParameter;

        internal CardFilter_Zone () { }

        internal CardFilter_Zone (ZoneGetter zoneGetter, Operation operation)
        {
            Operation = operation;
            zoneParameter = zoneGetter;
        }

        internal CardFilter_Zone (string tag)
        {
            Operation = Operation.Equals;
            simpleTag = tag;
        }

        internal override bool IsMatch (Card card)
        {
            if (!string.IsNullOrEmpty(simpleTag))
                return card.CurrentZone?.HasTag(simpleTag) ?? false;
            if (zoneParameter != null && card.CurrentZone != null)
                return zoneParameter.GetZones().Contains(card.CurrentZone);
            return false;
        }
    }

    [Serializable]
    internal class CardFilter_Amount : Filter<Card>
    {
        [JsonProperty]
        internal int amount;

        internal CardFilter_Amount () { }

        internal CardFilter_Amount (int amount)
        {
            this.amount = amount;
        }

        internal override bool IsMatch (Card card)
        {
            return card.CurrentZone.Count - card.index > amount;
        }
    }
}
