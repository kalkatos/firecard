using System;

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
            if (ReferenceEquals(a, null) || (ReferenceEquals(b, null)))
            {
                Logger.LogWarning($"Resolving two objects with null value: {a} {operation} {b}");
                return false; 
            }
            switch (Type.GetTypeCode(a.GetType()))
            {
                case TypeCode.Single:
                    if (IsNumeric(b) && (int)operation < 2)
                        return Resolve((float)a, operation, (float)b);
                    break;
                case TypeCode.String:
                    if (b is string)
                        return Resolve((string)a, operation, (string)b);
                    break;
                default:
                    if ((int)operation < 2)
                        return objResolvers[(int)operation](a, b);
                    break;
            }
            throw new ArgumentException($"Resolve cannot treat types: {a.GetType()} and {b.GetType()} with operation {operation}.");
        }

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

        private static Func<float, float, bool>[] floatResolvers = new Func<float, float, bool>[]
        {
            EqualsFuncF,
            NotEqualsFuncF,
            LessThanFuncF,
            LessOrEqualsFuncF,
            GreaterThanFuncF,
            GreaterOrEqualsFuncF,
        };

        private static Func<object, object, bool>[] objResolvers = new Func<object, object, bool>[]
        {
            EqualsFuncObj,
            NotEqualsFuncObj,
        };

        private static bool EqualsFuncObj (object a, object b) => a.Equals(b);
        private static bool NotEqualsFuncObj (object a, object b) => !a.Equals(b);
        private static bool EqualsFuncStr (string a, string b) => a == b;
        private static bool NotEqualsFuncStr (string a, string b) => a != b;
        private static bool EqualsFuncF (float a, float b) => a == b;
        private static bool NotEqualsFuncF (float a, float b) => a != b;
        private static bool LessThanFuncF (float a, float b) => a < b;
        private static bool LessOrEqualsFuncF (float a, float b) => a <= b;
        private static bool GreaterThanFuncF (float a, float b) => a > b;
        private static bool GreaterOrEqualsFuncF (float a, float b) => a >= b;
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
