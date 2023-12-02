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
        public static event Action<Card, Zone, Zone> OnCardEnteredZone;
        public static event Action<Card, Zone> OnCardLeftZone;

        internal static Random Random;

        private static List<Card> cards;
        private static List<Zone> zones;
        private static Dictionary<Trigger, List<Rule>> rules;
        private static List<string> phases;
        private static Dictionary<string, string> variables = new();
        private static MatchState currentState;

        private const string VAR_NAME = "variable";
        private const string VAR_VALUE = "newValue";
        private const string VAR_OLD_VALUE = "oldValue";
        private const string MOVED_CARD = "movedCard";
        private const string NEW_ZONE = "newZone";
        private const string OLD_ZONE = "oldZone";

        private static string[] defaultVariables = new string[]
        {
            "matchNumber", "turnNumber", "phase", "actionName", "message", VAR_NAME,
            VAR_VALUE, VAR_OLD_VALUE, "rule", "ruleName", "usedCard", "usedCardZone",
            MOVED_CARD, NEW_ZONE, OLD_ZONE, "usedZone", "additionalInfo", "this",
        };

        public static void Setup (MatchData matchData)
        {
            // Cards
            cards = new();
            for (int i = 0; i < matchData.Cards.Count; i++)
            {
                Card newCard = new Card(matchData.Cards[i]);
                cards.Add(newCard);
            }
            // Zones
            zones = new();
            for (int i = 0; i < matchData.Zones.Count; i++)
            {
                Zone newZone = new Zone(matchData.Zones[i]);
                zones.Add(newZone);
            }
            // Rules
            rules = new();
            foreach (Rule rule in matchData.Rules)
            {
                if (rules.ContainsKey(rule.Trigger))
                    rules[rule.Trigger].Add(rule);
                else
                    rules.Add(rule.Trigger, new List<Rule>() { rule });
            }
            // Phases
            phases = new List<string>(matchData.Phases);
            // Variables
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
            // Events
            OnVariableChanged += HandleVariableChanged;
            OnCardEnteredZone += HandleCardEnteredZone;
            OnCardLeftZone += HandleCardLeftZone;
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
                    List<Zone> zonesToShuffle = effect.ZoneParameter.GetZones();
                    for (int i = 0; i < zonesToShuffle.Count; i++)
                        zonesToShuffle[i].Shuffle();
                    break;
                case EffectType.UseCard:
                    break;
                case EffectType.UseZone:
                    break;
                case EffectType.MoveCardToZone:
                    List<Zone> zones = effect.ZoneParameter.GetZones();
                    foreach (Zone zone in zones)
                    {
                        List<Card> cards = effect.CardParameter.GetCards();
                        if (cards.Count > 0)
                        {
                            if (effect.MoveCardOption == MoveCardOption.ToBottom)
                                zone.InsertCards(cards, 0, OnCardEnteredZone, OnCardLeftZone);
                            else
                                zone.PushCards(cards, OnCardEnteredZone, OnCardLeftZone);
                        }
                    }
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

        public static MatchState GetState ()
        {
            // TODO Return actual current state
            return new MatchState();
        }

        private static void SetVariable (string varName, string varValue)
        {
            variables[varName] = varValue;
        }

        private static void TriggerRules (Trigger trigger)
        {
            if (rules.ContainsKey(trigger))
            {
                foreach (Rule rule in rules[trigger])
                {
                    rule.Condition.Evaluate(currentState);
                    if (rule.Condition.GetValue())
                    {
                        foreach (Effect effect in rule.TrueEffects)
                            ExecuteEffect(effect);
                    }
                    else
                    {
                        foreach (Effect effect in rule.FalseEffects)
                            ExecuteEffect(effect);
                    }
                }
            }
        }

        private static void HandleVariableChanged (string varName, string oldValue, string newValue)
        {
            SetVariable(VAR_NAME, varName);
            SetVariable(VAR_OLD_VALUE, oldValue);
            SetVariable(VAR_VALUE, newValue);
            TriggerRules(Trigger.OnVariableChanged);
        }

        private static void HandleCardEnteredZone (Card card, Zone newZone, Zone oldZone)
        {
            SetVariable(MOVED_CARD, card.id);
            SetVariable(NEW_ZONE, newZone.id);
            SetVariable(OLD_ZONE, oldZone.id);
            TriggerRules(Trigger.OnCardEnteredZone);
        }

        private static void HandleCardLeftZone (Card card, Zone oldZone)
        {
            SetVariable(MOVED_CARD, card.id);
            SetVariable(OLD_ZONE, oldZone.id);
            TriggerRules(Trigger.OnCardLeftZone);
        }
    }
}