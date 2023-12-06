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
        private static List<string> phases;
        private static List<Player> players;
        private static Dictionary<Trigger, List<Rule>> rules;
        private static Dictionary<string, string> variables;
        private static Dictionary<string, Card> cardsById;
        private static Dictionary<string, Zone> zonesById;
        private static Dictionary<string, Rule> rulesById;
        private static Dictionary<string, Player> playersById;
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
        private const string PLAYER = "player";

        private static string[] defaultVariables = new string[]
        {
            MATCH_NUMBER, TURN_NUMBER, PHASE, ACTION_NAME, MESSAGE, VAR_NAME,
            VAR_VALUE, VAR_OLD_VALUE, RULE, RULE_NAME, USED_CARD, USED_CARD_ZONE,
            MOVED_CARD, NEW_ZONE, OLD_ZONE, USED_ZONE, ADDITIONAL_INFO, THIS,
            PLAYER, 
        };

        public static void Setup (MatchData matchData)
        {
            if (matchData == null)
                throw new ArgumentException("Match data cannot be null.");
            Random = new Random();
            matchNumber = matchData.MatchNumber;
            // Cards
            cards = new();
            cardsById = new();
            if (matchData.Cards != null)
            {
                for (int i = 0; i < matchData.Cards.Count; i++)
                {
                    string newId;
                    do newId = "c" + Random.Next(1_000_000, 10_000_000);
                    while (cardsById.ContainsKey(newId));
                    Card newCard = new Card(matchData.Cards[i]);
                    newCard.id = newId;
                    cardsById.Add(newId, newCard);
                    cards.Add(newCard);
                }
            }
            // Zones
            zones = new();
            zonesById = new();
            if (matchData.Zones != null)
            {
                for (int i = 0; i < matchData.Zones.Count; i++)
                {
                    string newId;
                    do newId = "z" + Random.Next(10_000, 100_000);
                    while (zonesById.ContainsKey(newId));
                    Zone newZone = new Zone(matchData.Zones[i]);
                    newZone.id = newId;
                    zonesById.Add(newId, newZone);
                    zones.Add(newZone);
                }
            }
            // Rules
            rules = new();
            rulesById = new();
            if (matchData.Rules != null)
            { 
                foreach (Rule rule in matchData.Rules)
                {
                    string newId;
                    do newId = "r" + Random.Next(1_000_000, 10_000_000);
                    while (cardsById.ContainsKey(newId));
                    rule.id = newId;
                    rulesById.Add(newId, rule);
                    if (rules.ContainsKey(rule.Trigger))
                        rules[rule.Trigger].Add(rule);
                    else
                        rules.Add(rule.Trigger, new List<Rule>() { rule });
                    foreach (Effect effect in rule.TrueEffects)
                        RegisterEffectAction(effect);
                    foreach (Effect effect in rule.FalseEffects)
                        RegisterEffectAction(effect);
                }
            }
            // Phases
            phases = new();
            if (matchData.Phases != null)
                phases.AddRange(matchData.Phases);
            // Variables
            variables = new();
            for (int i = 0; i < defaultVariables.Length; i++)
                variables.Add(defaultVariables[i], "");
            if (matchData.Variables != null)
            {
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
            // Players
            players = new();
            playersById = new();
            if (matchData.Players != null)
            {
                for (int i = 0; i < matchData.Players.Count; i++)
                {
                    string newId;
                    do newId = "p" + Random.Next(10_000, 100_000);
                    while (playersById.ContainsKey(newId));
                    Player newPlayer = new Player(matchData.Players[i]);
                    newPlayer.id = newId;
                    playersById.Add(newId, newPlayer);
                    players.Add(newPlayer);
                }
            }
        }

        public static void Start ()
        {
            RaiseMatchStarted(matchNumber);
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
                    EndCurrentPhase_EffectAction(effect);
                    break;
                case EffectType.EndTheMatch:
                    EndTheMatch_EffectAction(effect);
                    break;
                case EffectType.EndSubphaseLoop:
                    EndSubphaseLoop_EffectAction(effect);
                    break;
                case EffectType.UseAction:
                    UseAction_EffectAction(effect);
                    break;
                case EffectType.StartSubphaseLoop:
                    StartSubphaseLoop_EffectAction(effect);
                    break;
                case EffectType.Shuffle:
                    Shuffle_EffectAction(effect);
                    break;
                case EffectType.UseCard:
                    UseCard_EffectAction(effect);
                    break;
                case EffectType.UseZone:
                    UseZone_EffectAction(effect);
                    break;
                case EffectType.MoveCardToZone:
                    MoveCardToZone_EffectAction(effect);
                    break;
                case EffectType.SetCardFieldValue:
                    SetCardFieldValue_EffectAction(effect);
                    break;
                case EffectType.SetVariable:
                    SetVariable_EffectAction(effect);
                    break;
                case EffectType.AddTagToCard:
                    AddTagToCard_EffectAction(effect);
                    break;
                case EffectType.RemoveTagFromCard:
                    RemoveTagFromCard_EffectAction(effect);
                    break;
                default:
                    throw new NotImplementedException("Effect type not implemented: " + effect.EffectType);
            }
        }

        public static void Dispose ()
        {
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
                    effect.ExecutionFunction = EndCurrentPhase_EffectAction;
                    break;
                case EffectType.EndTheMatch:
                    effect.ExecutionFunction = EndTheMatch_EffectAction;
                    break;
                case EffectType.EndSubphaseLoop:
                    effect.ExecutionFunction = EndSubphaseLoop_EffectAction;
                    break;
                case EffectType.UseAction:
                    effect.ExecutionFunction = UseAction_EffectAction;
                    break;
                case EffectType.StartSubphaseLoop:
                    effect.ExecutionFunction = StartSubphaseLoop_EffectAction;
                    break;
                case EffectType.Shuffle:
                    effect.ExecutionFunction = Shuffle_EffectAction;
                    break;
                case EffectType.UseCard:
                    effect.ExecutionFunction = UseCard_EffectAction;
                    break;
                case EffectType.UseZone:
                    effect.ExecutionFunction = UseZone_EffectAction;
                    break;
                case EffectType.MoveCardToZone:
                    effect.ExecutionFunction = MoveCardToZone_EffectAction;
                    break;
                case EffectType.SetCardFieldValue:
                    effect.ExecutionFunction = SetCardFieldValue_EffectAction;
                    break;
                case EffectType.SetVariable:
                    effect.ExecutionFunction = SetVariable_EffectAction;
                    break;
                case EffectType.AddTagToCard:
                    effect.ExecutionFunction = AddTagToCard_EffectAction;
                    break;
                case EffectType.RemoveTagFromCard:
                    effect.ExecutionFunction = RemoveTagFromCard_EffectAction;
                    break;
                default:
                    throw new NotImplementedException("Effect type not implemented: " + effect.EffectType);
            }
        }

        private static void EndCurrentPhase_EffectAction (Effect effect)
        {

        }

        private static void EndTheMatch_EffectAction (Effect effect)
        {

        }

        private static void EndSubphaseLoop_EffectAction (Effect effect)
        {

        }

        private static void UseAction_EffectAction (Effect effect)
        {

        }

        private static void StartSubphaseLoop_EffectAction (Effect effect)
        {

        }

        private static void Shuffle_EffectAction (Effect effect)
        {
            List<Zone> zonesToShuffle = effect.ZoneParameter.GetZones();
            for (int i = 0; i < zonesToShuffle.Count; i++)
                zonesToShuffle[i].Shuffle();
        }

        private static void UseCard_EffectAction (Effect effect)
        {

        }

        private static void UseZone_EffectAction (Effect effect)
        {

        }

        private static void MoveCardToZone_EffectAction (Effect effect)
        {
            List<Zone> zones = effect.ZoneParameter.GetZones();
            foreach (Zone zone in zones)
            {
                List<Card> cards = effect.CardParameter.GetCards();
                if (cards.Count > 0)
                {
                    if (effect.MoveCardOption == MoveCardOption.ToBottom)
                        zone.InsertCards(cards, 0, RaiseCardEnteredZone, RaiseCardLeftZone);
                    else
                        zone.PushCards(cards, RaiseCardEnteredZone, RaiseCardLeftZone);
                }
            }
        }

        private static void SetCardFieldValue_EffectAction (Effect effect)
        {

        }

        private static void SetVariable_EffectAction (Effect effect)
        {
            string varName = effect.StringParameter1.GetString();
            if (effect.NumberParameter != null)
            {
                float newValue = effect.NumberParameter.GetNumber();
                float oldValue = 0;
                if (variables.ContainsKey(varName))
                    float.TryParse(variables[varName], out oldValue);
                variables[varName] = newValue.ToString();
                RaiseVariableChanged(varName, oldValue.ToString(), newValue.ToString());
            }
            else if (effect.StringParameter2 != null)
            {

            }
        }

        private static void AddTagToCard_EffectAction (Effect effect)
        {

        }

        private static void RemoveTagFromCard_EffectAction (Effect effect)
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

        private static void RaiseMatchStarted (int matchNumber)
        {
            SetVariable(MATCH_NUMBER, matchNumber.ToString());
            TriggerRules(Trigger.OnMatchStarted);
            OnMatchStarted?.Invoke(matchNumber);
        }

        private static void RaiseMatchEnded (int matchNumber)
        {
            SetVariable(MATCH_NUMBER, matchNumber.ToString());
            TriggerRules(Trigger.OnMatchEnded);
            OnMatchEnded?.Invoke(matchNumber);
            Dispose();
        }

        private static void RaiseTurnStarted (int turnNumber)
        {
            SetVariable(TURN_NUMBER, turnNumber.ToString());
            TriggerRules(Trigger.OnTurnStarted);
            OnTurnStarted?.Invoke(turnNumber);
        }

        private static void RaiseTurnEnded (int turnNumber)
        {
            SetVariable(TURN_NUMBER, turnNumber.ToString());
            TriggerRules(Trigger.OnTurnEnded);
            OnTurnEnded?.Invoke(turnNumber);
        }

        private static void RaisePhaseStarted (string phase)
        {
            SetVariable(PHASE, phase);
            TriggerRules(Trigger.OnPhaseStarted);
            OnPhaseStarted?.Invoke(phase);
        }

        private static void RaisePhaseEnded (string phase)
        {
            SetVariable(PHASE, phase);
            TriggerRules(Trigger.OnPhaseEnded);
            OnPhaseEnded?.Invoke(phase);
        }

        private static void RaiseCardUsed (Card card)
        {
            SetVariable(USED_CARD, card.id);
            SetVariable(USED_CARD_ZONE, (card.CurrentZone != null ? card.CurrentZone.id : ""));
            TriggerRules(Trigger.OnCardUsed);
            OnCardUsed?.Invoke(card);
        }

        private static void RaiseZoneUsed (Zone zone)
        {
            SetVariable(USED_ZONE, zone.id);
            TriggerRules(Trigger.OnZoneUsed);
            OnZoneUsed?.Invoke(zone);
        }

        private static void RaiseCardEnteredZone (Card card, Zone newZone, Zone oldZone)
        {
            SetVariable(MOVED_CARD, card.id);
            SetVariable(NEW_ZONE, newZone.id);
            SetVariable(OLD_ZONE, oldZone.id);
            TriggerRules(Trigger.OnCardEnteredZone);
            OnCardEnteredZone?.Invoke(card, newZone, oldZone);
        }

        private static void RaiseCardLeftZone (Card card, Zone oldZone)
        {
            SetVariable(MOVED_CARD, card.id);
            SetVariable(OLD_ZONE, oldZone.id);
            TriggerRules(Trigger.OnCardLeftZone);
            OnCardLeftZone?.Invoke(card, oldZone);
        }

        private static void RaiseActionUsed (string actionName)
        {
            SetVariable(ACTION_NAME, actionName);
            TriggerRules(Trigger.OnActionUsed);
            OnActionUsed?.Invoke(actionName);
        }

        private static void RaiseVariableChanged (string varName, string oldValue, string newValue)
        {
            SetVariable(VAR_NAME, varName);
            SetVariable(VAR_OLD_VALUE, oldValue);
            SetVariable(VAR_VALUE, newValue);
            TriggerRules(Trigger.OnVariableChanged);
            OnVariableChanged?.Invoke(varName, oldValue, newValue);
        }

        private static void RaiseRuleActivated (Rule rule)
        {
            SetVariable(RULE, rule.id);
            SetVariable(RULE_NAME, rule.Name);
            TriggerRules(Trigger.OnRuleActivated);
            OnRuleActivated?.Invoke(rule);
        }
    }
}