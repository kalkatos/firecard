﻿using System;
using System.Text;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Abstract class for something that gets information.
    /// </summary>
    [Serializable]
    public abstract class Getter
    {
        public abstract object Get ();
    }

    public struct Variable
    {
        public string Value;
        public static Variable USED_CARD = new Variable() { Value = "usedCard" };
        public static Variable USED_ZONE = new Variable() { Value = "usedZone" };
        public static Variable USED_CARD_ZONE = new Variable() { Value = "usedCardZone" };
        public static Variable USED_ACTION = new Variable() { Value = "usedAction" };
        public static Variable NEW_ZONE = new Variable() { Value = "newZone" };
        public static Variable OLD_ZONE = new Variable() { Value = "oldZone" };
    }

    public struct Tag
    {
        public string Value;

        public const string FACE_DOWN = "FaceDown";

        public static Tag C (string cardTag) => new Tag() { Value = cardTag };

        public static Tag Z (string zoneTag) => new Tag() { Value = zoneTag };

        public static Tag Or (params string[] tags)
        {
            if (tags == null || tags.Length == 0)
                throw new ArgumentException("Tag.Or need a list of strings to be concatenated.");
            StringBuilder sb = new();
            sb.Append(tags[0]);
            for (int i = 1; i < tags.Length; i++)
            {
                sb.Append("|");
                sb.Append(tags[i]); 
            }
            return new Tag() { Value = sb.ToString() };
        }

        public static Tag And (params string[] tags)
        {
            if (tags == null || tags.Length == 0)
                throw new ArgumentException("Tag.And need a list of strings to be concatenated.");
            StringBuilder sb = new();
            sb.Append(tags[0]);
            for (int i = 1; i < tags.Length; i++)
            {
                sb.Append("&");
                sb.Append(tags[i]);
            }
            return new Tag() { Value = sb.ToString() };
        }

        public static Tag Not (params string[] tags)
        {
            if (tags == null || tags.Length == 0)
                throw new ArgumentException("Tag.And need a list of strings to be concatenated.");
            StringBuilder sb = new();
            sb.Append("!");
            sb.Append(tags[0]);
            for (int i = 1; i < tags.Length; i++)
            {
                sb.Append("!");
                sb.Append("&");
                sb.Append(tags[i]);
            }
            return new Tag() { Value = sb.ToString() };
        }
    }
}
