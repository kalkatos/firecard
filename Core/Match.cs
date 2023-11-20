using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Main controller of this framework. When executing, it will run through the list of rules and execute their effects if the corresponding trigger is active and the condition is met.
    /// </summary>
    [Serializable]
    public class Match
    {
        public static event Action<string, string, string> OnVariableChanged;

        internal static Random Random;

        private static List<Card> cards;
        private static List<Zone> zones;
        private static List<Rule> rules;
        private static List<string> phases;
        private static Dictionary<string, string> variables = new();

        private static string[] defaultVariables = new string[]
        {
            "matchNumber", "turnNumber", "phase", "actionName", "message", "variable",
            "newValue", "oldValue", "rule", "ruleName", "usedCard", "movedCard",
            "newZone", "oldZone", "usedZone", "additionalInfo", "this",
        };

        public static void Setup (MatchData matchData)
        {
            cards = new();
            for (int i = 0; i < matchData.Cards.Count; i++)
            {
                Card newCard = new Card(matchData.Cards[i]);
                cards.Add(newCard);
            }
            zones = new();
            for (int i = 0; i < matchData.Zones.Count; i++)
            {
                Zone newZone = new Zone(matchData.Zones[i]);
                zones.Add(newZone);
            }
            rules = new List<Rule>(matchData.Rules);
            phases = new List<string>(matchData.Phases);
            variables = new();
            for (int i = 0; i < defaultVariables.Length; i++)
                variables.Add(defaultVariables[i], "");
            for (int i = 0; i < matchData.Variables.Count; i++)
            {
                string variable = matchData.Variables[i].Item1;
                if (Array.IndexOf(defaultVariables, variable) >= 0)
                {
                    Logger.LogWarning($"Variable {variable} is a reserved match variable and can not be included.");
                    continue;
                }
                if (variables.ContainsKey(variable))
                {
                    Logger.LogWarning($"Duplicate variable name: {variable}.");
                    continue;
                }
                variables.Add(variable, matchData.Variables[i].Item2);
            }
        }

        public static float GetNumericVariable (string variableName)
        {
            if (variables.TryGetValue(variableName, out string value)
                && float.TryParse(value, out float parsedValue))
                return parsedValue;
            return float.NaN;
        }

        public static string GetStringVariable (string variableName)
        {
            if (variables.TryGetValue(variableName, out string value))
                return value;
            return null;
        }

        public static void ExecuteEffect (Effect effect)
        {
            switch (effect.EffectType)
            {
                case EffectType.EndCurrentPhase:
                    break;
                case EffectType.EndTheMatch:
                    break;
                case EffectType.EndSubphaseLoop:
                    break;
                case EffectType.UseAction:
                    break;
                case EffectType.StartSubphaseLoop:
                    break;
                case EffectType.Shuffle:
                    break;
                case EffectType.UseCard:
                    break;
                case EffectType.UseZone:
                    break;
                case EffectType.MoveCardToZone:
                    break;
                case EffectType.SetCardFieldValue:
                    break;
                case EffectType.SetVariable:
                    string varName = effect.StringParameter1.GetString();
                    if (effect.NumberParameter != null)
                    {
                        float newValue = effect.NumberParameter.GetNumber();
                        float oldValue = 0;
                        if (variables.ContainsKey(varName))
                            float.TryParse(variables[varName], out oldValue);
                        variables[varName] = newValue.ToString();
                        OnVariableChanged?.Invoke(varName, oldValue.ToString(), newValue.ToString());
                    }
                    else if (effect.StringParameter2 != null)
                    {

                    }
                    break;
                case EffectType.AddTagToCard:
                    break;
                case EffectType.RemoveTagFromCard:
                    break;
                default:
                    throw new NotImplementedException("Effect type not implemented: " + effect.EffectType);
            }
        }
    }
}