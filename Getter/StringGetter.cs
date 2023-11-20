using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Gets and returns a string information from different sources.
    /// </summary>
    [Serializable]
    public class StringGetter : Getter
    {
        public StringGetterType Type;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StringGetter SubStringGetter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CardGetter CardGetter;

        [JsonIgnore]
        private string value;

        [JsonIgnore]
        public string Value => GetString();

        public StringGetter () { }

        public StringGetter (string value)
        {
            Type = StringGetterType.Plain;
            this.value = value;
        }

        public StringGetter (StringGetterType type, string value)
        {
            if (type == StringGetterType.FieldString)
                throw new ArgumentException("A string getter with only a string as parameter must be of type VariableString.");
            Type = type;
            if (type == StringGetterType.Plain)
                this.value = value;
            else if (type == StringGetterType.VariableString)
                SubStringGetter = new StringGetter(value);
        }

        public StringGetter (StringGetter variableName)
        {
            Type = StringGetterType.VariableString;
            SubStringGetter = variableName;
        }

        public StringGetter (StringGetter fieldName, CardGetter cardGetter)
        {
            Type = StringGetterType.FieldString;
            SubStringGetter = fieldName;
            CardGetter = cardGetter;
        }

        public string GetString ()
        {
            switch (Type)
            {
                case StringGetterType.Plain:
                    return value;
                case StringGetterType.VariableString:
                    return Match.GetStringVariable(SubStringGetter.GetString());
                case StringGetterType.FieldString:
                    Card[] cards = CardGetter.GetCards();
                    if (cards != null && cards.Length > 0)
                        return cards[0].GetStringFieldValue(SubStringGetter.GetString());
                    return null;
                default:
                    throw new NotImplementedException("StringGetterType not implemented: " + Type);
            }
        }

        public override object Get ()
        {
            return GetString();
        }
    }

    public enum StringGetterType
    {
        Plain,
        VariableString,
        FieldString,
    }
}
