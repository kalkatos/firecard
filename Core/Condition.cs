using Newtonsoft.Json;
using System;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Checks information on match and return True or False according to internal setup.
    /// </summary>
    [Serializable]
    public class Condition : IValueGetter<bool>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Left;
        public Operation Operator;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Right;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Condition SubCondition;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Condition AndCondition;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Condition OrCondition;

        private bool value = true;
        private Condition end = null;

        [JsonIgnore]
        public static string[] Operators = new string[] { "=", "!=", "<", "<=", ">", ">=", "->", "!>", "<>" };

        public Condition (string expression, params object[] values)
        {
            throw new NotImplementedException("Condition with expression not implemented.");
        }

        public Condition (object left, string operation, object right)
        {
            int operationIndex = Array.IndexOf(Operators, operation);
            if (operationIndex < 0)
                throw new ArgumentException($"Operation is wrong ({operation}), expected: =, !=, <, <=, >, >=, ->, !>, <> ");
            Left = left;
            Operator = (Operation)operationIndex;
            Right = right;
            end = this;
        }

        public Condition (object left, Operation operation, object right)
        {
            Left = left;
            Operator = operation;
            Right = right;
            end = this;
        }

        public Condition (Condition sub)
        {
            SubCondition = sub;
            end = this;
        }

        public Condition And (object left, string @operator, object right)
        {
            AddAnd(new Condition(left, @operator, right));
            return this;
        }

        public Condition And (Condition andCondition)
        {
            AddAnd(new Condition(andCondition));
            return this;
        }

        public Condition Or (object left, string @operator, object right)
        {
            AddOr(new Condition(left, @operator, right));
            return this;
        }

        public Condition Or (Condition orCondition)
        {
            AddOr(new Condition(orCondition));
            return this;
        }

        public bool GetValue ()
        {
            if (SubCondition != null)
                value = SubCondition.GetValue();
            if (AndCondition != null)
                return value && AndCondition.GetValue();
            if (OrCondition != null)
                return value || OrCondition.GetValue();
            return value;
        }

        public void Evaluate (MatchState matchState)
        {
            AndCondition?.Evaluate(matchState);
            OrCondition?.Evaluate(matchState);
            if (SubCondition != null)
            {
                SubCondition.Evaluate(matchState);
                value = SubCondition.value;
                return;
            }
            // TODO Set own value based on what matchState contains
        }

        public override string ToString ()
        {
            string result;
            if (SubCondition != null)
                result = $"({SubCondition})";
            else
                result = Left + Operators[(int)Operator] + Right;
            if (AndCondition != null)
                result += $" AND {AndCondition}";
            if (OrCondition != null)
                result += $" OR {OrCondition}";
            return result;
        }

        private void AddAnd (Condition newCondition)
        {
            end.AndCondition = newCondition;
            end = newCondition;
        }

        private void AddOr (Condition newCondition)
        {
            end.OrCondition = newCondition;
            end = newCondition;
        }
    }

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
        HasAll = 8,
    }
}