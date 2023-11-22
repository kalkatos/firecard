using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        [JsonProperty]
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
                    break;
                case NumberGetterType.CardCount:
                    value = CardGetter.GetCards().Count;
                    break;
                case NumberGetterType.ZoneCount:
                    value = ZoneGetter.GetZones().Count;
                    break;
                case NumberGetterType.FieldNumeric:
                    List<Card> cards = CardGetter.GetCards();
                    if (cards != null && cards.Count > 0)
                        value = cards[0].GetNumericFieldValue(StringGetter.GetString());
                    else
                        value = float.NaN;
                    break;
                case NumberGetterType.VariableNumeric:
                    value = Match.GetNumericVariable(StringGetter.GetString());
                    break;
                case NumberGetterType.CardIndex:
                    List<Card> cards2 = CardGetter.GetCards();
                    if (cards2 != null && cards2.Count > 0)
                        value = cards2[0].Index;
                    else
                        value =  float.NaN;
                    break;
                case NumberGetterType.RandomInt:
                    value = Match.Random.Next((int)NumberGetter1.GetNumber(), (int)NumberGetter2.GetNumber());
                    break;
                case NumberGetterType.RandomFloat:
                    float f1 = NumberGetter1.GetNumber(), f2 = NumberGetter2.GetNumber();
                    value =  f1 + (f2 - f1) * (float)Match.Random.NextDouble();
                    break;
                default:
                    throw new NotImplementedException("NumberGetterType not implemented: " + Type);
            }
            return value;
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
