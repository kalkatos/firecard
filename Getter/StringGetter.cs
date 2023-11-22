using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public StringGetter StringParameter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CardGetter CardParameter;

        [JsonProperty]
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
                StringParameter = new StringGetter(value);
        }

        public StringGetter (StringGetter variableName)
        {
            Type = StringGetterType.VariableString;
            StringParameter = variableName;
        }

        public StringGetter (StringGetter fieldName, CardGetter cardGetter)
        {
            Type = StringGetterType.FieldString;
            StringParameter = fieldName;
            CardParameter = cardGetter;
        }

        public string GetString ()
        {
            switch (Type)
            {
                case StringGetterType.Plain:
                    break;
                case StringGetterType.VariableString:
                    value = Match.GetStringVariable(StringParameter.GetString());
                    break;
                case StringGetterType.FieldString:
                    List<Card> cards = CardParameter.GetCards();
                    if (cards != null && cards.Count > 0)
                        value = cards[0].GetStringFieldValue(StringParameter.GetString());
                    else
                        value = "";
                    break;
                default:
                    throw new NotImplementedException("StringGetterType not implemented: " + Type);
            }
            return value;
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
