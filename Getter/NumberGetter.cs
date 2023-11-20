using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Gets and returns a number from different sources.
    /// </summary>
    [Serializable]
    public class NumberGetter : Getter
    {
        public NumberGetterType Type;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StringGetter StringGetter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NumberGetter NumberGetter1;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NumberGetter NumberGetter2;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CardGetter CardGetter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ZoneGetter ZoneGetter;

        [JsonIgnore]
        private float value;

        [JsonIgnore]
        public float Value => GetNumber();

        public NumberGetter () { }

        public NumberGetter (float value)
        {
            Type = NumberGetterType.Plain;
            this.value = value;
        }

        public NumberGetter (ZoneGetter zoneGetter)
        {
            Type = NumberGetterType.ZoneCount;
            ZoneGetter = zoneGetter;
        }

        public NumberGetter (NumberGetterType type, CardGetter cardGetter)
        {
            if (type != NumberGetterType.CardCount
                && type != NumberGetterType.CardIndex)
                throw new ArgumentException("A number getter that uses a card getter must be of type CardCount or CardIndex.");
            Type = type;
            CardGetter = cardGetter;
        }

        public NumberGetter (int min, int max)
        {
            Type = NumberGetterType.RandomInt;
            NumberGetter1 = new NumberGetter(min);
            NumberGetter2 = new NumberGetter(max);
        }

        public NumberGetter (float min, float max)
        {
            Type = NumberGetterType.RandomFloat;
            NumberGetter1 = new NumberGetter(min);
            NumberGetter2 = new NumberGetter(max);
        }

        public NumberGetter (NumberGetter min, int max)
        {
            Type = NumberGetterType.RandomInt;
            NumberGetter1 = min;
            NumberGetter2 = new NumberGetter(max);
        }

        public NumberGetter (NumberGetter min, float max)
        {
            Type = NumberGetterType.RandomFloat;
            NumberGetter1 = min;
            NumberGetter2 = new NumberGetter(max);
        }

        public NumberGetter (int min, NumberGetter max)
        {
            Type = NumberGetterType.RandomInt;
            NumberGetter1 = new NumberGetter(min);
            NumberGetter2 = max;
        }

        public NumberGetter (float min, NumberGetter max)
        {
            Type = NumberGetterType.RandomFloat;
            NumberGetter1 = new NumberGetter(min);
            NumberGetter2 = max;
        }

        public NumberGetter (NumberGetterType type, NumberGetter min, NumberGetter max)
        {
            if (type != NumberGetterType.RandomInt
                && type != NumberGetterType.RandomFloat)
                throw new ArgumentException("A number getter that gets a random number must be of type RandomInt or RandomFloat.");
            Type = type;
            NumberGetter1 = min;
            NumberGetter2 = max;
        }

        public NumberGetter (CardGetter cardGetter, StringGetter fieldName)
        {
            Type = NumberGetterType.FieldNumeric;
            CardGetter = cardGetter;
            StringGetter = fieldName;
        }

        public NumberGetter (StringGetter variableName)
        {
            Type = NumberGetterType.VariableNumeric;
            StringGetter = variableName;
        }

        public float GetNumber ()
        {
            switch (Type)
            {
                case NumberGetterType.Plain:
                    return value;
                case NumberGetterType.CardCount:
                    return CardGetter.GetCards().Length;
                case NumberGetterType.ZoneCount:
                    return ZoneGetter.GetZones().Length;
                case NumberGetterType.FieldNumeric:
                    Card[] cards = CardGetter.GetCards();
                    if (cards != null && cards.Length > 0)
                        return cards[0].GetNumericFieldValue(StringGetter.GetString());
                    return float.NaN;
                case NumberGetterType.VariableNumeric:
                    return Match.GetNumericVariable(StringGetter.GetString());
                case NumberGetterType.CardIndex:
                    Card[] cards2 = CardGetter.GetCards();
                    if (cards2 != null && cards2.Length > 0)
                        return cards2[0].Index;
                    return float.NaN;
                case NumberGetterType.RandomInt:
                    return Match.Random.Next((int)NumberGetter1.GetNumber(), (int)NumberGetter2.GetNumber());
                case NumberGetterType.RandomFloat:
                    float f1 = NumberGetter1.GetNumber(), f2 = NumberGetter2.GetNumber();
                    return f1 + (f2 - f1) * (float)Match.Random.NextDouble();
                default:
                    throw new NotImplementedException("NumberGetterType not implemented: " + Type);
            }
        }

        public override object Get ()
        {
            return GetNumber();
        }
    }

    public enum NumberGetterType
    {
        Plain,
        CardCount,
        ZoneCount,
        FieldNumeric,
        VariableNumeric,
        CardIndex,
        RandomInt,
        RandomFloat,
    }
}
