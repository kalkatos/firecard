using System;
using Kalkatos.Firecard.Utility;
using Newtonsoft.Json;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Holds information on changes to be executed on Match. Used in ExecuteEffect method in the Match class.
    /// </summary>
    [Serializable]
    public class Effect
    {
        public EffectType EffectType;
        public MoveCardOption MoveCardOption;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StringGetter StringParameter1;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StringGetter StringParameter2;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NumberGetter NumberParameter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CardGetter CardParameter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ZoneGetter ZoneParameter;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Getter GetterParameter;

        [JsonIgnore]
        public Action<Effect> ExecutionFunction;

        public Effect ()
        {
            ExecutionFunction = ExecuteSelf;
        }

        public static Effect EndCurrentPhase ()
        {
            return new Effect { EffectType = EffectType.EndCurrentPhase };
        }

        public static Effect EndSubphaseLoop ()
        {
            return new Effect { EffectType = EffectType.EndSubphaseLoop };
        }

        public static Effect EndTheMatch ()
        {
            return new Effect { EffectType = EffectType.EndTheMatch };
        }

        public static Effect UseAction (string actionName)
        {
            return new Effect { EffectType = EffectType.UseAction, StringParameter1 = new StringGetter(actionName) };
        }

        public static Effect UseAction (StringGetter actionName)
        {
            return new Effect { EffectType = EffectType.UseAction, StringParameter1 = actionName };
        }

        public static Effect StartSubphaseLoop (string phases)
        {
            return new Effect { EffectType = EffectType.StartSubphaseLoop, StringParameter1 = new StringGetter(phases) };
        }

        public static Effect StartSubphaseLoop (StringGetter phases)
        {
            return new Effect { EffectType = EffectType.StartSubphaseLoop, StringParameter1 = phases };
        }

        public static Effect Shuffle (ZoneGetter zoneGetter)
        {
            return new Effect { EffectType = EffectType.Shuffle, ZoneParameter = zoneGetter };
        }

        public static Effect UseCard (CardGetter cardGetter)
        {
            return new Effect { EffectType = EffectType.UseCard, CardParameter = cardGetter };
        }

        public static Effect UseZone (ZoneGetter zoneGetter)
        {
            return new Effect { EffectType = EffectType.UseZone, ZoneParameter = zoneGetter };
        }

        public static Effect MoveCardToZone (CardGetter cardGetter, ZoneGetter zoneGetter, MoveCardOption option = MoveCardOption.None)
        {
            return new Effect { EffectType = EffectType.MoveCardToZone, CardParameter = cardGetter, ZoneParameter = zoneGetter, MoveCardOption = option };
        }

        public static Effect SetCardFieldValue (CardGetter cardGetter, string fieldName, Getter value)
        {
            return new Effect { EffectType = EffectType.SetCardFieldValue, CardParameter = cardGetter, StringParameter1 = new StringGetter(fieldName), GetterParameter = value };
        }

        public static Effect SetCardFieldValue (CardGetter cardGetter, StringGetter fieldName, Getter value)
        {
            return new Effect { EffectType = EffectType.SetCardFieldValue, CardParameter = cardGetter, StringParameter1 = fieldName, GetterParameter = value };
        }

        public static Effect SetVariable (StringGetter variableName, NumberGetter value)
        {
            return new Effect { EffectType = EffectType.SetVariable, StringParameter1 = variableName, NumberParameter = value };
        }

        public static Effect SetVariable (StringGetter variableName, StringGetter value)
        {
            return new Effect { EffectType = EffectType.SetVariable, StringParameter1 = variableName, StringParameter2 = value };
        }

        public static Effect AddTagToCard (StringGetter tag, CardGetter cardSelector)
        {
            return new Effect { EffectType = EffectType.AddTagToCard, StringParameter1 = tag, CardParameter = cardSelector };
        }

        public static Effect RemoveTagFromCard (StringGetter tag, CardGetter cardSelector)
        {
            return new Effect { EffectType = EffectType.RemoveTagFromCard, StringParameter1 = tag, CardParameter = cardSelector };
        }

        public static Effect RemoveTagFromCard (string tag, CardGetter cardSelector)
        {
            return new Effect { EffectType = EffectType.RemoveTagFromCard, StringParameter1 = new StringGetter(tag), CardParameter = cardSelector };
        }

        public static Effect RemoveTagFromCard (Tag tag, CardGetter cardSelector)
        {
            return new Effect { EffectType = EffectType.RemoveTagFromCard, StringParameter1 = new StringGetter(tag.Value), CardParameter = cardSelector };
        }

        private void ExecuteSelf (Effect effect)
        {
            Match.ExecuteEffect(this);
        }

        // TODO Finish effect simple builders

        /*
        EndCurrentPhase         ()
        EndSubphaseLoop         ()
        EndTheMatch             ()
        UseAction               (string actionName                                                           )
        StartSubphaseLoop       (string phases                                                               )
        Shuffle                 (ZoneSelector zoneSelector                                                   )
        UseCard                 (CardSelector cardSelector                                                   )
        UseZone                 (ZoneSelector zoneSelector                                                   )
        MoveCardToZone          (CardSelector cardSelector , ZoneSelector zoneSelector                       )
        SetCardFieldValue       (CardSelector cardSelector , string fieldName           , Getter value       )
        SetVariable             (string variableName       , Getter valueGetter                              )
        AddTagToCard            (CardSelector cardSelector , string tag                                      )
        RemoveTagFromCard       (CardSelector cardSelector , string tag                                      )
        // SendMessage          (string message                                                              )

        Getter can be:
        - Variable
        - Simple number
        - Math expression
        - Card selection count
        - 

        */
    }

    public enum EffectType
    {
        EndCurrentPhase,
        EndTheMatch,
        EndSubphaseLoop,
        UseAction,
        StartSubphaseLoop,
        Shuffle,
        UseCard,
        UseZone,
        MoveCardToZone,
        SetCardFieldValue,
        SetVariable,
        AddTagToCard,
        RemoveTagFromCard,
        //SendMessage,
    }

    public enum MoveCardOption
    {
        None,
        FaceDown,
        RevealedToOwner,
        FromBottom,
        ToBottom,
    }
}