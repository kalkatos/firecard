using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;

namespace Kalkatos.Firecard.Utility
{
    [Serializable]
    internal class Filter<T>
    {
        [JsonProperty]
        internal Operation Operation;
        internal virtual bool IsMatch (T zone) { return false; }
    }
}
