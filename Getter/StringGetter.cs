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
        [JsonProperty]
        private string value;

        [JsonIgnore]
        public string Value => value;

        public StringGetter () { }

        public StringGetter (string value)
        {
            this.value = value;
        }

        public string GetString ()
        {
            return value;
        }

        public override object Get ()
        {
            return GetString();
        }
    }
}
