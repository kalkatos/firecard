using Kalkatos.Firecard.Utility;
using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// The card is the core data holder for all gameplay. It moves between zones and holds tags and fields with information about the game. 
    /// </summary>
    [Serializable]
    public class Card
    {
        public event Action OnSetup;
        public event Action<Field> OnFieldChanged;

        /// <summary>
        /// Set by the Match when instantiated
        /// </summary>
        internal int id;
        /// <summary>
        /// Set by the zone the card is in
        /// </summary>
        internal int index;
        internal Zone currentZone;
        internal string name;
        internal List<string> tags;
        internal List<Field> fields;

        public int Index => index;
        public string Name => name;
        public Zone Zone => currentZone;
        public IReadOnlyList<string> Tags => tags.AsReadOnly();
        public IReadOnlyList<Field> Fields => fields.AsReadOnly();

        public void Setup (CardData cardData)
        {
            name = cardData.Name;
            tags = new List<string>(cardData.Tags);
            fields = new List<Field>(cardData.Fields);
            OnSetup?.Invoke();
        }

        public float GetNumericFieldValue (string fieldName)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Name == fieldName)
                    return fields[i].NumericValue;
            }
            return float.NaN;
        }

        public float GetNumericFieldValue (StringGetter fieldName)
        {
            return GetNumericFieldValue(fieldName.GetString());
        }

        public string GetStringFieldValue (string fieldName)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Name == fieldName)
                    return fields[i].StringValue;
            }
            return null;
        }

        public string GetStringFieldValue (StringGetter fieldName)
        {
            return GetStringFieldValue(fieldName.GetString());
        }

        internal void SetNumericValue (string fieldName, float value)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Name == fieldName)
                {
                    Field newField = new Field(fields[i]);
                    newField.NumericValue = value;
                    fields[i] = newField;
                    return;
                }
            }
        }

        internal void SetStringValue (string fieldName, string value)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Name == fieldName)
                {
                    Field newField = new Field(fields[i]);
                    newField.StringValue = value;
                    fields[i] = newField;
                    return;
                }
            }
        }
    }
}