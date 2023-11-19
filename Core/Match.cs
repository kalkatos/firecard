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

        private static Dictionary<string, string> variables = new();

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