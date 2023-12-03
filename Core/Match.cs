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
        public static event Action<int> OnMatchStarted;
        public static event Action<int> OnMatchEnded;
        public static event Action<int> OnTurnStarted;
        public static event Action<int> OnTurnEnded;
        public static event Action<string> OnPhaseStarted;
        public static event Action<string> OnPhaseEnded;
        public static event Action<Card> OnCardUsed;
        public static event Action<Zone> OnZoneUsed;
        public static event Action<Card, Zone, Zone> OnCardEnteredZone;
        public static event Action<Card, Zone> OnCardLeftZone;
        public static event Action<string> OnActionUsed;
        public static event Action<string, string, string> OnVariableChanged;
        public static event Action<Rule> OnRuleActivated;

        internal static Random Random;

        private static int matchNumber;
        private static List<Card> cards;
        private static List<Zone> zones;
        private static Dictionary<Trigger, List<Rule>> rules;
        private static List<string> phases;
        private static Dictionary<string, string> variables;
        private static MatchState currentState;

        private const string MATCH_NUMBER = "matchNumber";
        private const string TURN_NUMBER = "turnNumber";
        private const string PHASE = "phase";
        private const string ACTION_NAME = "actionName";
        private const string MESSAGE = "message";
        private const string VAR_NAME = "variable";
        private const string VAR_VALUE = "newValue";
        private const string VAR_OLD_VALUE = "oldValue";
        private const string RULE = "rule";
        private const string RULE_NAME = "ruleName";
        private const string USED_CARD = "usedCard";
        private const string USED_CARD_ZONE = "usedCardZone";
        private const string MOVED_CARD = "movedCard";
        private const string NEW_ZONE = "newZone";
        private const string OLD_ZONE = "oldZone";
        private const string USED_ZONE = "usedZone";
        private const string ADDITIONAL_INFO = "additionalInfo";
        private const string THIS = "this";

        private static string[] defaultVariables = new string[]
        {
            MATCH_NUMBER, TURN_NUMBER, PHASE, ACTION_NAME, MESSAGE, VAR_NAME,
            VAR_VALUE, VAR_OLD_VALUE, RULE, RULE_NAME, USED_CARD, USED_CARD_ZONE,
            MOVED_CARD, NEW_ZONE, OLD_ZONE, USED_ZONE, ADDITIONAL_INFO, THIS,
        };

        public static void Setup (MatchData matchData)
        {
            Random = new Random();
            matchNumber = matchData.MatchNumber;
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
                foreach (Effect effect in rule.TrueEffects)
                    RegisterEffectAction(effect);
                foreach (Effect effect in rule.FalseEffects)
                    RegisterEffectAction(effect);
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
            OnMatchStarted += HandleMatchStarted;
            OnMatchEnded += HandleMatchEnded;
            OnTurnStarted += HandleTurnStarted;
            OnTurnEnded += HandleTurnEnded;
            OnPhaseStarted += HandlePhaseStarted;
            OnPhaseEnded += HandlePhaseEnded;
            OnCardUsed += HandleCardUsed;
            OnZoneUsed += HandleZoneUsed;
            OnCardEnteredZone += HandleCardEnteredZone;
            OnCardLeftZone += HandleCardLeftZone;
            OnActionUsed += HandleActionUsed;
            OnVariableChanged += HandleVariableChanged;
            OnRuleActivated += HandleRuleActivated;
        }

        public static void Start ()
        {
            OnMatchStarted?.Invoke(matchNumber);
        }

        public static MatchState GetState ()
        {
            // TODO Return actual current state
            return new MatchState();
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
                    EndCurrentPhaseEffectAction(effect);
                    break;
                case EffectType.EndTheMatch:
                    EndTheMatchEffectAction(effect);
                    break;
                case EffectType.EndSubphaseLoop:
                    EndSubphaseLoopEffectAction(effect);
                    break;
                case EffectType.UseAction:
                    UseActionEffectAction(effect);
                    break;
                case EffectType.StartSubphaseLoop:
                    StartSubphaseLoopEffectAction(effect);
                    break;
                case EffectType.Shuffle:
                    ShuffleEffectAction(effect);
                    break;
                case EffectType.UseCard:
                    UseCardEffectAction(effect);
                    break;
                case EffectType.UseZone:
                    UseZoneEffectAction(effect);
                    break;
                case EffectType.MoveCardToZone:
                    MoveCardToZoneEffectAction(effect);
                    break;
                case EffectType.SetCardFieldValue:
                    SetCardFieldValueEffectAction(effect);
                    break;
                case EffectType.SetVariable:
                    SetVariableEffectAction(effect);
                    break;
                case EffectType.AddTagToCard:
                    AddTagToCardEffectAction(effect);
                    break;
                case EffectType.RemoveTagFromCard:
                    RemoveTagFromCardEffectAction(effect);
                    break;
                default:
                    throw new NotImplementedException("Effect type not implemented: " + effect.EffectType);
            }
        }

        public static void Dispose ()
        {
            OnVariableChanged -= HandleVariableChanged;
            OnCardEnteredZone -= HandleCardEnteredZone;
            OnCardLeftZone -= HandleCardLeftZone;
            OnMatchStarted -= HandleMatchStarted;
            OnMatchEnded -= HandleMatchEnded;
            OnTurnStarted -= HandleTurnStarted;
            OnTurnEnded -= HandleTurnEnded;
            OnPhaseStarted -= HandlePhaseStarted;
            OnPhaseEnded -= HandlePhaseEnded;
            OnCardUsed -= HandleCardUsed;
            OnZoneUsed -= HandleZoneUsed;
            OnCardEnteredZone -= HandleCardEnteredZone;
            OnCardLeftZone -= HandleCardLeftZone;
            OnActionUsed -= HandleActionUsed;
            OnVariableChanged -= HandleVariableChanged;
            OnRuleActivated -= HandleRuleActivated;
            cards = null;
            zones = null;
            rules = null;
            phases = null;
            variables = null;
            currentState = null;
        }

        private static void RegisterEffectAction (Effect effect)
        {
            switch (effect.EffectType)
            {
                case EffectType.EndCurrentPhase:
                    effect.ExecutionFunction = EndCurrentPhaseEffectAction;
                    break;
                case EffectType.EndTheMatch:
                    effect.ExecutionFunction = EndTheMatchEffectAction;
                    break;
                case EffectType.EndSubphaseLoop:
                    effect.ExecutionFunction = EndSubphaseLoopEffectAction;
                    break;
                case EffectType.UseAction:
                    effect.ExecutionFunction = UseActionEffectAction;
                    break;
                case EffectType.StartSubphaseLoop:
                    effect.ExecutionFunction = StartSubphaseLoopEffectAction;
                    break;
                case EffectType.Shuffle:
                    effect.ExecutionFunction = ShuffleEffectAction;
                    break;
                case EffectType.UseCard:
                    effect.ExecutionFunction = UseCardEffectAction;
                    break;
                case EffectType.UseZone:
                    effect.ExecutionFunction = UseZoneEffectAction;
                    break;
                case EffectType.MoveCardToZone:
                    effect.ExecutionFunction = MoveCardToZoneEffectAction;
                    break;
                case EffectType.SetCardFieldValue:
                    effect.ExecutionFunction = SetCardFieldValueEffectAction;
                    break;
                case EffectType.SetVariable:
                    effect.ExecutionFunction = SetVariableEffectAction;
                    break;
                case EffectType.AddTagToCard:
                    effect.ExecutionFunction = AddTagToCardEffectAction;
                    break;
                case EffectType.RemoveTagFromCard:
                    effect.ExecutionFunction = RemoveTagFromCardEffectAction;
                    break;
                default:
                    throw new NotImplementedException("Effect type not implemented: " + effect.EffectType);
            }
        }

        private static void EndCurrentPhaseEffectAction (Effect effect)
        {

        }

        private static void EndTheMatchEffectAction (Effect effect)
        {

        }

        private static void EndSubphaseLoopEffectAction (Effect effect)
        {

        }

        private static void UseActionEffectAction (Effect effect)
        {

        }

        private static void StartSubphaseLoopEffectAction (Effect effect)
        {

        }

        private static void ShuffleEffectAction (Effect effect)
        {
            List<Zone> zonesToShuffle = effect.ZoneParameter.GetZones();
            for (int i = 0; i < zonesToShuffle.Count; i++)
                zonesToShuffle[i].Shuffle();
        }

        private static void UseCardEffectAction (Effect effect)
        {

        }

        private static void UseZoneEffectAction (Effect effect)
        {

        }

        private static void MoveCardToZoneEffectAction (Effect effect)
        {
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
        }

        private static void SetCardFieldValueEffectAction (Effect effect)
        {

        }

        private static void SetVariableEffectAction (Effect effect)
        {
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
        }

        private static void AddTagToCardEffectAction (Effect effect)
        {

        }

        private static void RemoveTagFromCardEffectAction (Effect effect)
        {

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
                            effect.ExecutionFunction.Invoke(effect);
                    }
                    else
                    {
                        foreach (Effect effect in rule.FalseEffects)
                            effect.ExecutionFunction.Invoke(effect);
                    }
                }
            }
        }

        private static void HandleMatchStarted (int matchNumber)
        {
            SetVariable(MATCH_NUMBER, matchNumber.ToString());
            TriggerRules(Trigger.OnMatchStarted);
        }

        private static void HandleMatchEnded (int matchNumber)
        {
            Dispose();
        }

        private static void HandleTurnStarted (int turnNumber)
        {
            
        }

        private static void HandleTurnEnded (int turnNumber)
        {

        }

        private static void HandlePhaseStarted (string phase)
        {

        }

        private static void HandlePhaseEnded (string phase)
        {

        }

        private static void HandleCardUsed (Card card)
        {

        }

        private static void HandleZoneUsed (Zone zone)
        {

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

        private static void HandleActionUsed (string actionName)
        {

        }

        private static void HandleVariableChanged (string varName, string oldValue, string newValue)
        {
            SetVariable(VAR_NAME, varName);
            SetVariable(VAR_OLD_VALUE, oldValue);
            SetVariable(VAR_VALUE, newValue);
            TriggerRules(Trigger.OnVariableChanged);
        }

        private static void HandleRuleActivated (Rule rule)
        {

        }
    }
}