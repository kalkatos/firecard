using System;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Checks information on match and return True or False according to internal setup.
    /// </summary>
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
            Contains = 6,
            NotContains = 7,
        }
        public static string[] Operators = new string[] { "=", "!=", "<", "<=", ">", ">=", "->", "!>" };

        public bool GetValue ()
        {
            if (Sub != null)
                value = Sub.GetValue();
            if (And != null)
                return value && And.GetValue();
            if (Or != null)
                return value || Or.GetValue();
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
                result = Left + Operators[(int)Operator] + Right;
            if (And != null)
                result += $" AND {And}";
            if (Or != null)
                result += $" OR {Or}";
            return result;
        }
    }
}