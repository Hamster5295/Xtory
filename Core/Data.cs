using System.Collections.Generic;

namespace Xtory
{
    public enum DataType
    {
        Int, Float, Bool, String, Other
    }

    public static class DataUtils
    {
        private static readonly HashSet<string> trueStr = new() { "true", "t", "yes", "y", "是", "对" };
        private static readonly HashSet<string> falseStr = new() { "false", "f", "no", "n", "否" };

        public static DataType GetObjectType(object obj)
        {
            if (obj is int) return DataType.Int;
            else if (obj is float) return DataType.Float;
            else if (obj is bool) return DataType.Bool;
            else if (obj is string) return DataType.String;
            else return DataType.Other;
        }

        public static DataType CastString(string obj, out object result)
        {
            if (trueStr.Contains(obj.ToLower())) { result = true; return DataType.Bool; }
            else if (falseStr.Contains(obj.ToLower())) { result = false; return DataType.Bool; }
            else if (int.TryParse(obj, out var intVal)) { result = intVal; return DataType.Int; }
            else if (float.TryParse(obj, out var floatVal)) { result = floatVal; return DataType.Float; }
            else { result = obj; return DataType.String; }
        }

        public static bool GetCompareType(DataType ta, DataType tb, out DataType compareType)
        {
            if (ta == tb)
            {
                compareType = ta;
                return true;
            }
            else if (ta == DataType.Int && tb == DataType.Float || ta == DataType.Float && tb == DataType.Int)
            {
                compareType = DataType.Float;
                return true;
            }
            compareType = DataType.Other;
            return false;
        }
    }
}