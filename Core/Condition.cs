using System;

namespace Kalkatos.Firecard.Core
{
    [Serializable]
    public class Condition : IValueGetter<bool>
    {
        public string Left;
        public Operation Operator;
        public string Right;
        public Condition Sub;
        public Condition And;
        public Condition Or;

        private bool value;

        public enum Operation
        {
            Equals = 0,
            NotEquals = 1,
            LessThan = 2,
            LessOrEquals = 3,
            GreaterThan = 4,
            GreaterOrEquals = 5,
        }
        public static string[] Operators = new string[] { "=", "!=", "<", "<=", ">", ">=" };

        public bool Value => GetValue();
        public string Face => Left + Operators[(int)Operator] + Right;

        public bool GetValue ()
        {
            if (Sub != null)
                value = Sub.Value;
            if (And != null)
                return value && And.Value;
            if (Or != null)
                return value || Or.Value;
            return value;
        }

        public void Evaluate (MatchState matchState)
        {
            And?.Evaluate(matchState);
            Or?.Evaluate(matchState);
            if (Sub != null)
            {
                Sub.Evaluate(matchState);
                value = Sub.value;
                return;
            }
            // TODO Set own value based on what matchState contains
        }

        public override string ToString ()
        {
            string result;
            if (Sub != null)
                result = $"({Sub})";
            else
                result = Face;
            if (And != null)
                result += $" AND {And}";
            if (Or != null)
                result += $" OR {Or}";
            return result;
        }
    }
}