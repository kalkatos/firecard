using Kalkatos.Firecard.Core;
using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Utility
{
    internal static class Evaluator
    {
        internal static string[] Operators = new string[] { "=", "!=", "<", "<=", ">", ">=" };

        internal static Operation StringToOperation (string operationStr)
        {
            int operationIndex = Array.IndexOf(Operators, operationStr);
            if (operationIndex < 0)
                throw new ArgumentException($"Operation is wrong ({operationStr}), expected: =, !=, <, <=, >, >=");
            return (Operation)operationIndex;
        }

        internal static string OperationToString (Operation operation)
        {
            return Operators[(int)operation];
        }

        internal static bool Resolve (string a, Operation operation, string b)
        {
            return stringResolvers[(int)operation](a, b);
        }

        internal static bool Resolve (float a, Operation operation, float b)
        {
            return floatResolvers[(int)operation](a, b);
        }

        internal static bool Resolve (object a, Operation operation, object b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                Logger.LogWarning($"Resolving two objects with null value: {a} {operation} {b}");
                return false; 
            }
            if (IsNumeric(a))
            {
                if (IsNumeric(b))
                    return floatResolvers[(int)operation]((float)a, (float)b);
            }
            else if (a is string)
            {
                if (b is string)
                    return stringResolvers[(int)operation]((string)a, (string)b);
            }
            else
                return objResolvers[(int)operation](a, b);
            throw new ArgumentException($"Resolve cannot treat types: {a.GetType()} and {b.GetType()} with operation {operation}.");
        }

        internal static bool Resolve (Card card, Operation operation, string tag)
        {
            return cardTagResolvers[(int)operation](card, tag);
        }

        internal static bool Resolve (string tag, Operation operation, Card card)
        {
            return Resolve(card, operation, tag);
        }

        internal static bool Resolve (List<Card> cards, Operation operation, string tag)
        {
            for (int i = 0; i < cards.Count; i++)
                if (!Resolve(cards[i], operation, tag))
                    return false;
            return true;
        }

        internal static bool Resolve (string tag, Operation operation, List<Card> cards)
        {
            return Resolve(cards, operation, tag);
        }

        internal static bool Resolve (List<Card> cards, Operation operation, Card card)
        {
            return listContainsCardResolvers[(int)operation](cards, card);
        }

        internal static bool Resolve (Card card, Operation operation, List<Card> cards)
        {
            return Resolve(cards, operation, card);
        }

        internal static bool Resolve (List<Card> first, Operation operation, List<Card> second)
        {
            return cardListContainsListResolvers[(int)operation](first, second);
        }

        internal static bool Resolve (Zone zone, Operation operation, string tag)
        {
            return zoneTagResolvers[(int)operation](zone, tag);
        }
        
        internal static bool Resolve (string tag, Operation operation, Zone zone)
        {
            return Resolve(zone, operation, tag);
        }

        internal static bool Resolve (List<Zone> zones, Operation operation, string tag)
        {
            for (int i = 0; i < zones.Count; i++)
                if (!Resolve(zones[i], operation, tag))
                    return false;
            return true;
        }

        internal static bool Resolve (string tag, Operation operation, List<Zone> zones)
        {
            return Resolve(zones, operation, tag);
        }

        internal static bool Resolve (List<Zone> zones, Operation operation, Zone zone)
        {
            return listContainsZoneResolvers[(int)operation](zones, zone);
        }

        internal static bool Resolve (Zone zone, Operation operation, List<Zone> zones)
        {
            return Resolve(zones, operation, zone);
        }

        internal static bool Resolve (List<Zone> first, Operation operation, List<Zone> second)
        {
            return zoneListContainsListResolvers[(int)operation](first, second);
        }

        /*

        CardGetter  =  !=  Tag
        ZoneGetter  =  !=  Tag
        FieldGetter  =  !=  string
        string  =  !=  FieldGetter
        StringGetter  =  !=  StringGetter
        StringGetter  =  !=  string
        string  =  !=  StringGetter
        StringGetter  =  !=  Tag
        Tag  =  !=  StringGetter
        string  =  !=  Tag
        Tag  =  !=  string
        Zone  =  !=  string
        string  =  !=  Zone
        Card  =  !=  string
        string  =  !=  Card
        Card  =  !=  StringGetter
        StringGetter  =  !=  Card
        Zone  =  !=  StringGetter
        StringGetter  =  !=  Zone

        NumberGetter  numOp  number
        NumberGetter  numOp  NumberGetter
        number  numOp  NumberGetter
        FieldGetter  numOp  FieldGetter
        FieldGetter  numOp  number
        FieldGetter  numOp  NumberGetter
        number  numOp  FieldGetter
        NumberGetter  numOp  FieldGetter

        CardGetter  =  !=  CardGetter
        CardGetter  =  !=  Card
        Card  =  !=  CardGetter

        ZoneGetter  =  !=  ZoneGetter
        ZoneGetter  =  !=  Zone
        Zone  =  !=   ZoneGetter

        */

        private static bool IsNumeric (object obj)
        {
            TypeCode typeCode = Type.GetTypeCode(obj.GetType());
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        private static Func<string, string, bool>[] stringResolvers = new Func<string, string, bool>[]
        {
            EqualsFuncStr,
            NotEqualsFuncStr,
        };
        private static bool EqualsFuncStr (string a, string b) => a == b;
        private static bool NotEqualsFuncStr (string a, string b) => a != b;

        private static Func<float, float, bool>[] floatResolvers = new Func<float, float, bool>[]
        {
            EqualsFuncF,
            NotEqualsFuncF,
            LessThanFuncF,
            LessOrEqualsFuncF,
            GreaterThanFuncF,
            GreaterOrEqualsFuncF,
        };
        private static bool EqualsFuncF (float a, float b) => a == b;
        private static bool NotEqualsFuncF (float a, float b) => a != b;
        private static bool LessThanFuncF (float a, float b) => a < b;
        private static bool LessOrEqualsFuncF (float a, float b) => a <= b;
        private static bool GreaterThanFuncF (float a, float b) => a > b;
        private static bool GreaterOrEqualsFuncF (float a, float b) => a >= b;

        private static Func<object, object, bool>[] objResolvers = new Func<object, object, bool>[]
        {
            EqualsFuncObj,
            NotEqualsFuncObj,
        };
        private static bool EqualsFuncObj (object a, object b) => a.Equals(b);
        private static bool NotEqualsFuncObj (object a, object b) => !a.Equals(b);

        private static Func<Card, string, bool>[] cardTagResolvers = new Func<Card, string, bool>[]
        {
            EqualsFuncCardTag,
            NotEqualsFuncCardTag,
        };
        private static bool EqualsFuncCardTag (Card card, string tag) => card.HasTag(tag);
        private static bool NotEqualsFuncCardTag (Card card, string tag) => !card.HasTag(tag);

        private static Func<List<Card>, Card, bool>[] listContainsCardResolvers = new Func<List<Card>, Card, bool>[]
        {
            EqualsFuncListContainsCard,
            NotEqualsFuncListDoesNotContainCard,
        };
        private static bool EqualsFuncListContainsCard (List<Card> list, Card card) => list.Contains(card);
        private static bool NotEqualsFuncListDoesNotContainCard (List<Card> list, Card card) => !list.Contains(card);

        private static Func<List<Card>, List<Card>, bool>[] cardListContainsListResolvers = new Func<List<Card>, List<Card>, bool>[]
        {
            EqualsFuncListContainsCard,
            NotEqualsFuncListDoesNotContainCard,
        };
        private static bool EqualsFuncListContainsCard (List<Card> first, List<Card> second)
        {
            for (int i = 0; i < second.Count; i++)
                if (!first.Contains(second[i]))
                    return false;
            return true;
        }
        private static bool NotEqualsFuncListDoesNotContainCard (List<Card> first, List<Card> second) => !EqualsFuncListContainsCard(first, second);

        private static Func<Zone, string, bool>[] zoneTagResolvers = new Func<Zone, string, bool>[]
        {
            EqualsFuncZoneTag,
            NotEqualsFuncZoneTag,
        };
        private static bool EqualsFuncZoneTag (Zone zone, string tag) => zone.HasTag(tag);
        private static bool NotEqualsFuncZoneTag (Zone zone, string tag) => !zone.HasTag(tag);

        private static Func<List<Zone>, Zone, bool>[] listContainsZoneResolvers = new Func<List<Zone>, Zone, bool>[]
        {
            EqualsFuncListContainsZone,
            NotEqualsFuncListDoesNotContainZone,
        };
        private static bool EqualsFuncListContainsZone (List<Zone> list, Zone zone) => list.Contains(zone);
        private static bool NotEqualsFuncListDoesNotContainZone (List<Zone> list, Zone zone) => !list.Contains(zone);

        private static Func<List<Zone>, List<Zone>, bool>[] zoneListContainsListResolvers = new Func<List<Zone>, List<Zone>, bool>[]
        {
            EqualsFuncListContainsZone,
            NotEqualsFuncListDoesNotContainZone,
        };
        private static bool EqualsFuncListContainsZone (List<Zone> first, List<Zone> second)
        {
            for (int i = 0; i < second.Count; i++)
                if (!first.Contains(second[i]))
                    return false;
            return true;
        }
        private static bool NotEqualsFuncListDoesNotContainZone (List<Zone> first, List<Zone> second) => !EqualsFuncListContainsZone(first, second);
    }

    public enum Operation
    {
        Equals = 0,
        NotEquals = 1,
        LessThan = 2,
        LessOrEquals = 3,
        GreaterThan = 4,
        GreaterOrEquals = 5,
    }
}
