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

        public static Effect EndCurrentPhase ()
        {
            return new Effect { EffectType = EffectType.EndCurrentPhase };
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

        public static Effect Shuffle (ZoneGetter zoneGetter)
        {
            return new Effect { EffectType = EffectType.Shuffle, ZoneParameter = zoneGetter };
        }

        public static Effect MoveCardToZone (CardGetter cardGetter, ZoneGetter zoneGetter, MoveCardOption option = MoveCardOption.None)
        {
            return new Effect { EffectType = EffectType.MoveCardToZone, CardParameter = cardGetter, ZoneParameter = zoneGetter, MoveCardOption = option };
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
        ToBottom,
    }
}