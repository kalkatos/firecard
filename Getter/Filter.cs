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
        internal virtual bool IsMatch (T obj) { return false; }

        protected Func<string, string, bool>[] stringResolvers = new Func<string, string, bool>[]
        {
            EqualsFuncStr,
            NotEqualsFuncStr,
        };

        protected Func<float, float, bool>[] floatResolvers = new Func<float, float, bool>[]
        {
            EqualsFuncF,
            NotEqualsFuncF,
            LessThanFuncF,
            LessOrEqualsFuncF,
            GreaterThanFuncF,
            GreaterOrEqualsFuncF,
        };

        protected bool Resolve (string a, string b)
        {
            return stringResolvers[(int)Operation](a, b);
        }

        protected bool Resolve (float a, float b)
        {
            return floatResolvers[(int)Operation](a, b);
        }

        protected static bool EqualsFuncStr (string a, string b) => a == b;
        protected static bool NotEqualsFuncStr (string a, string b) => a != b;
        protected static bool EqualsFuncF (float a, float b) => a == b;
        protected static bool NotEqualsFuncF (float a, float b) => a != b;
        protected static bool LessThanFuncF (float a, float b) => a < b;
        protected static bool LessOrEqualsFuncF (float a, float b) => a <= b;
        protected static bool GreaterThanFuncF (float a, float b) => a > b;
        protected static bool GreaterOrEqualsFuncF (float a, float b) => a >= b;
    }
}
