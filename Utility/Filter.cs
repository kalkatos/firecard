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

        internal virtual void Prepare () { }
        internal virtual bool IsMatch (T obj) { return false; }

        protected bool Resolve (string a, string b)
        {
            return Evaluator.Resolve(a, Operation, b);
        }

        protected bool Resolve (float a, float b)
        {
            return Evaluator.Resolve(a, Operation, b);
        }

        protected bool Resolve (object a, object b)
        {
            return Evaluator.Resolve(a, Operation, b);
        }
    }
}
