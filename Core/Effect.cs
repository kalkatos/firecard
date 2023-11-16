using System;
using Kalkatos.Firecard.Utility;

namespace Kalkatos.Firecard.Core
{
    [Serializable]
    public class Effect
    {
        public EffectType EffectType;
        public string StringParameter;
        public CardSelector CardParameter;
        public ZoneSelector ZoneParameter;
        public Getter GetterParameter;

        public static Effect EndCurrentPhase ()
        {
            return new Effect { EffectType = EffectType.EndCurrentPhase };
        }

        public static Effect AddTagToCard (string tag, CardSelector cardSelector)
        {
            return new Effect { EffectType = EffectType.AddTagToCard, CardParameter = cardSelector };
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
}