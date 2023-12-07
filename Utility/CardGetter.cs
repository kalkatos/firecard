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

        public CardGetter Id (string id, Operation operation)
        {
            Filters.Add(new CardFilter_Id(id, operation));
            return this;
        }

        public CardGetter Id (string id)
        {
            return Id(id, Operation.Equals);
        }

        public CardGetter Id (Variable variable, Operation operation)
        {
            Filters.Add(new CardFilter_IdFromVariable(variable, operation));
            return this;
        }

        public CardGetter Id (Variable variable)
        {
            Filters.Add(new CardFilter_IdFromVariable(variable, Operation.Equals));
            return this;
        }

        public CardGetter Zone (string tag, Operation operation)
        {
            Filters.Add(new CardFilter_Zone(tag, operation));
            return this;
        }

        public CardGetter Zone (string tag)
        {
            return Zone(tag, Operation.Equals);
        }

        public CardGetter Zone (ZoneGetter zoneGetter, Operation operation)
        {
            Filters.Add(new CardFilter_ZoneGetter(zoneGetter, operation));
            return this;
        }

        public CardGetter Zone (Variable variable)
        {
            return Zone(new ZoneGetter().Id(Match.GetStringVariable(variable.Value)));
        }

        public CardGetter Zone (ZoneGetter zoneGetter)
        {
            return Zone(zoneGetter, Operation.Equals);
        }

        public CardGetter Zone (CardGetter other, Operation operation)
        {
            Filters.Add(new CardFilter_ZoneCard(other, operation));
            return this;
        }

        public CardGetter Zone (CardGetter other)
        {
            return Zone(other, Operation.Equals);
        }

        public CardGetter Tag (ZoneGetter zoneGetter, Operation operation)
        {
            Filters.Add(new CardFilter_ZoneGetter(zoneGetter, operation));
            return this;
        }

        public CardGetter Tag (string tag, Operation operation)
        {
            Filters.Add(new CardFilter_Tag(tag, operation));
            return this;
        }

        public CardGetter Tag (string tag)
        {
            return Tag(tag, Operation.Equals);
        }

        public CardGetter Tag (StringGetter tag, Operation operation)
        {
            Filters.Add(new CardFilter_TagGetter(tag, operation));
            return this;
        }

        public CardGetter Tag (StringGetter tag)
        {
            return Tag(tag, Operation.Equals);
        }

        public CardGetter Field (string fieldName, Getter getter, Operation operation)
        {
            Filters.Add(new CardFilter_Field(fieldName, getter, operation));
            return this;
        }

        public CardGetter Field (string fieldName, Getter getter)
        {
            return Field(fieldName, getter, Operation.Equals);
        }

        public CardGetter Field (string fieldName, string value, Operation operation)
        {
            Filters.Add(new CardFilter_FieldText(fieldName, value, operation));
            return this;
        }

        public CardGetter Field (string fieldName, string value)
        {
            return Field(fieldName, value, Operation.Equals);
        }

        public CardGetter Top (int value)
        {
            Filters.Add(new CardFilter_Top(value));
            return this;
        }

        public CardGetter Bottom (int value)
        {
            Filters.Add(new CardFilter_Bottom(value));
            return this;
        }

        public CardGetter Index (NumberGetter numberGetter, Operation operation)
        {
            Filters.Add(new CardFilter_Index(numberGetter, operation));
            return this;
        }

        public CardGetter Index (string operation, NumberGetter numberGetter)
        {
            return Index(numberGetter, Evaluator.StringToOperation(operation));
        }

        public CardGetter Visibility (int value, Operation operation)
        {
            Filters.Add(new CardFilter_Visibility(value, operation));
            return this;
        }

        public CardGetter Visibility (int value)
        {
            return Visibility(value, Operation.Equals);
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
            if (cards.Count == 0 || Filters.Count == 0)
                return;
            for (int i = 0; i < Filters.Count; i++)
                Filters[i].Prepare();
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
            return Evaluator.Resolve(card.id, Operation, plainString) || Evaluator.Resolve(card.id, Operation, Match.GetStringVariable(plainString));
        }
    }

    [Serializable]
    internal class CardFilter_IdFromVariable : Filter<Card>
    {
        [JsonProperty]
        internal Variable variable;

        internal CardFilter_IdFromVariable () { }

        internal CardFilter_IdFromVariable (Variable variable, Operation operation)
        {
            Operation = operation;
            this.variable = variable;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card.id, Operation, Match.GetStringVariable(variable.Value));
        }
    }

    [Serializable]
    internal class CardFilter_Tag : Filter<Card>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal string simpleTag;

        internal CardFilter_Tag () { }

        internal CardFilter_Tag (string tag, Operation operation) 
        {
            simpleTag = tag;
            Operation = operation;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card, Operation, simpleTag);
        }
    }

    [Serializable]
    internal class CardFilter_TagGetter : Filter<Card>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal StringGetter stringGetter;

        internal CardFilter_TagGetter () { }

        internal CardFilter_TagGetter (StringGetter stringGetter, Operation operation)
        {
            this.stringGetter = stringGetter;
            Operation = operation;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card, Operation, stringGetter.GetString());
        }
    }

    [Serializable]
    internal class CardFilter_Zone : Filter<Card>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal string simpleTag;

        internal CardFilter_Zone () { }

        internal CardFilter_Zone (string tag, Operation operation)
        {
            Operation = operation;
            simpleTag = tag;
        }

        internal override bool IsMatch (Card card)
        {
            if (card.CurrentZone == null)
                return false;
            return Evaluator.Resolve(card.CurrentZone, Operation, simpleTag);  // TODO Comparison between Zone and string if has tag
        }
    }

    [Serializable]
    internal class CardFilter_ZoneGetter : Filter<Card>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal ZoneGetter zoneGetter;

        private List<Zone> zoneSelection;

        internal CardFilter_ZoneGetter () { }

        internal CardFilter_ZoneGetter (ZoneGetter zoneGetter, Operation operation)
        {
            Operation = operation;
            this.zoneGetter = zoneGetter;
        }

        internal override void Prepare ()
        {
            if (zoneGetter != null)
                zoneSelection = zoneGetter.GetZones();
            else
                Logger.LogWarning("CardFilter_ZoneGetter zoneGetter is null.");
        }

        internal override bool IsMatch (Card card)
        {
            if (card.CurrentZone == null || zoneSelection == null)
                return false;
            return zoneSelection.Contains(card.CurrentZone);
        }
    }

    [Serializable]
    internal class CardFilter_ZoneCard : Filter<Card>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal CardGetter cardGetter;

        private Func<Card, bool> isMatchFuncToBeUsed;
        private Card cardReference;

        internal CardFilter_ZoneCard () { }

        internal CardFilter_ZoneCard (CardGetter cardGetter, Operation operation)
        {
            Operation = operation;
            this.cardGetter = cardGetter;
        }

        internal override void Prepare ()
        {
            if (cardGetter != null)
            {
                var selection = cardGetter.GetCards();
                if (selection.Count > 0)
                {
                    cardReference = selection[0];
                    if (cardReference.CurrentZone != null)
                    {
                        isMatchFuncToBeUsed = IsMatchByCardReference;
                        return;
                    }
                }
            }

            isMatchFuncToBeUsed = (c) => false;
        }

        internal override bool IsMatch (Card card)
        {
            return isMatchFuncToBeUsed.Invoke(card);
        }

        internal bool IsMatchByCardReference (Card card)
        {
            return Evaluator.Resolve(card.CurrentZone, Operation, cardReference.CurrentZone);
        }
    }

    [Serializable]
    internal class CardFilter_Top : Filter<Card>
    {
        [JsonProperty]
        internal int amount;

        internal CardFilter_Top () { }

        internal CardFilter_Top (int amount)
        {
            this.amount = amount;
        }

        internal override bool IsMatch (Card card)
        {
            return card.CurrentZone.Count - card.Index >= amount;
        }
    }

    [Serializable]
    internal class CardFilter_Bottom : Filter<Card>
    {
        [JsonProperty]
        internal int amount;

        internal CardFilter_Bottom () { }

        internal CardFilter_Bottom (int amount)
        {
            this.amount = amount;
        }

        internal override bool IsMatch (Card card)
        {
            return card.Index < amount;
        }
    }

    [Serializable]
    internal class CardFilter_Field : Filter<Card>
    {
        [JsonProperty]
        internal string fieldName;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal Getter getter;

        internal CardFilter_Field () { }

        internal CardFilter_Field (string fieldName, Getter getter, Operation operation)
        {
            Operation = operation;
            this.fieldName = fieldName;
            this.getter = getter;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card.GetFieldValue(fieldName), Operation, getter.Get());
        }
    }

    [Serializable]
    internal class CardFilter_FieldText : Filter<Card>
    {
        [JsonProperty]
        internal string fieldName;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal string value;

        internal CardFilter_FieldText () { }

        internal CardFilter_FieldText (string fieldName, string value, Operation operation)
        {
            Operation = operation;
            this.fieldName = fieldName;
            this.value = value;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card.GetTextFieldValue(fieldName), Operation, value);
        }
    }

    [Serializable]
    internal class CardFilter_Index : Filter<Card>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal NumberGetter numberGetter;

        internal CardFilter_Index () { }

        internal CardFilter_Index (NumberGetter numberGetter, Operation operation)
        {
            Operation = operation;
            this.numberGetter = numberGetter;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card.Index, Operation, numberGetter.GetNumber());
        }
    }

    [Serializable]
    internal class CardFilter_Visibility : Filter<Card>
    {
        internal int value;

        internal CardFilter_Visibility () { }

        internal CardFilter_Visibility (int value, Operation operation)
        {
            Operation = operation;
            this.value = value;
        }

        internal override bool IsMatch (Card card)
        {
            return Evaluator.Resolve(card.visibility, Operation, value);
        }
    }
}
