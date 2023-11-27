using Kalkatos.Firecard.Utility;
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
        public Operation Operation;
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

        public Condition (string expression, params object[] values)
        {
            throw new NotImplementedException("Condition with expression not implemented.");

            /*
            // When implemented, will be used like this
            Condition = new("1 != 2 & 3 = 4 & ( ( 5 = 6 & 7 = 8 ) | ( 9 = 10 & 11 = 12 ) )",
            // Condition = new("# != # & # = # & ( ( # = # & # = # ) | ( # = # & # = # ) )", // Alternative
                        Card.Id(Variable.USED_CARD),
                        Tag.FACE_DOWN,
                        Zone.Id(Variable.USED_ZONE),
                        Tag.Z("Foundation"),
                        Card.Id(Variable.USED_CARD),
                        Tag.C("Ace"),
                        NumberGetter.New(Card.Zone("Foundation").Field("Suit", Field.Getter("Suit", Card.Id(Variable.USED_CARD)))),
                        0,
                        Field.Getter("Suit", Card.Id(Variable.USED_CARD)),
                        Field.Getter("Suit", Card.Zone(Variable.USED_ZONE).Top(1)),
                        NumberGetter.Exp("# - #", Field.Getter("Value", Card.Id(Variable.USED_CARD)), Field.Getter("Value", Card.Zone(Variable.USED_ZONE).Top(1))),
                        1),
            */
        }

        public Condition (object left, string operation, object right)
        {
            Operation = Evaluator.StringToOperation(operation);
            Left = left;
            Right = right;
            end = this;
        }

        public Condition (object left, Operation operation, object right)
        {
            Left = left;
            Operation = operation;
            Right = right;
            end = this;
        }

        public Condition (Condition sub)
        {
            SubCondition = sub;
            end = this;
        }

        public Condition And (object left, string operation, object right)
        {
            AddAnd(new Condition(left, operation, right));
            return this;
        }

        public Condition And (Condition andCondition)
        {
            AddAnd(new Condition(andCondition));
            return this;
        }

        public Condition Or (object left, string operation, object right)
        {
            AddOr(new Condition(left, operation, right));
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
            value = Evaluator.Resolve(Left, Operation, Right);
        }

        public override string ToString ()
        {
            string result;
            if (SubCondition != null)
                result = $"( {SubCondition} )";
            else
                result = $"{Left} {Evaluator.OperationToString(Operation)} {Right}";
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
}