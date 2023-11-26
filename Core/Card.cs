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
        public static event Action<CardData> OnCardSetup;

        public event Action<CardData> OnSetup;
        public event Action<Field, Field> OnFieldChanged;

        /// <summary>
        /// Set by the Match when instantiated
        /// </summary>
        internal string id;
        /// <summary>
        /// Set by the zone the card is in
        /// </summary>
        internal int index;
        internal Zone currentZone;
        internal string name;
        internal List<string> tags;
        //internal List<Field> fields;

        private Dictionary<string, Field> fields = new();

        public int Index => index;
        public string Name => name;
        public Zone CurrentZone => currentZone;

        public Card () { }

        public Card (CardData cardData) : this()
        {
            Setup(cardData);
        }

        public void Setup (CardData cardData)
        {
            name = cardData.Name;
            tags = new List<string>(cardData.Tags);
            List<Field> dataFields = cardData.Fields;
            for (int i = 0; i < dataFields.Count; i++)
                fields.Add(dataFields[i].Name, dataFields[i]);
            OnSetup?.Invoke(cardData);
            OnCardSetup?.Invoke(cardData);
        }

        public bool HasTag (string tag)
        {
            return tags.Contains(tag);
        }

        public bool HasField (string fieldName)
        {
            return fields.ContainsKey(fieldName);
        }

        public object GetFieldValue (string fieldName)
        {
            if (!HasField(fieldName))
                return null;
            if (fields[fieldName].IsNumber())
                return fields[fieldName].Number;
            return fields[fieldName].Text;
        }

        public float GetNumericFieldValue (string fieldName)
        {
            if (!HasField(fieldName))
                return float.NaN;
            return fields[fieldName].Number.HasValue ? fields[fieldName].Number.Value : float.NaN;
        }

        public float GetNumericFieldValue (StringGetter fieldName)
        {
            return GetNumericFieldValue(fieldName.GetString());
        }

        public string GetTextFieldValue (string fieldName)
        {
            if (!HasField(fieldName))
                return null;
            return fields[fieldName].Text;
        }

        public string GetStringFieldValue (StringGetter fieldName)
        {
            return GetTextFieldValue(fieldName.GetString());
        }

        internal void SetNumericValue (string fieldName, float value)
        {
            Field oldField = fields.ContainsKey(fieldName) ? fields[fieldName] : new Field() { Name = fieldName };
            Field newField = new Field(oldField);
            newField.Number = value;
            fields[fieldName] = newField;
            OnFieldChanged?.Invoke(oldField, newField);
        }

        internal void SetStringValue (string fieldName, string value)
        {
            Field oldField = fields.ContainsKey(fieldName) ? fields[fieldName] : new Field() { Name = fieldName };
            Field newField = new Field(oldField);
            newField.Text = value;
            fields[fieldName] = newField;
            OnFieldChanged?.Invoke(oldField, newField);
        }

        public static CardGetter All => new CardGetter();

        public static CardGetter Tag (string tag)
        {
            return new CardGetter().Tag(tag);
        }

        public static CardGetter Tag (Tag tag)
        {
            return new CardGetter().Tag(tag.Value);
        }

        public static CardGetter Zone (string tag)
        {
            return new CardGetter().Zone(tag);
        }

        public static CardGetter Zone (Tag tag)
        {
            return new CardGetter().Zone(tag.Value);
        }

        public static CardGetter Zone (Variable variable)
        {
            return new CardGetter().Zone(variable);
        }

        public static CardGetter Zone (CardGetter cardGetter)
        {
            return new CardGetter().Zone(cardGetter);
        }

        public static CardGetter Id (Variable variable)
        {
            return new CardGetter().Id(variable);
        }
    }
}