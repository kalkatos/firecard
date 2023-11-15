using System;

namespace Kalkatos.Firecard.Core
{
    [Serializable]
    public class Condition : IValueGetter<bool>
    {
        private bool value;
        private Condition sub;
        private Condition and;
        private Condition or;

        public bool Value => GetValue();

        public bool GetValue ()
        {
            if (sub != null)
                return sub.Value;
            if (and != null)
                return value && and.Value;
            if (or != null)
                return value || or.Value;
            return value;
        }
    }
}