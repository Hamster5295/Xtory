using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xtory
{
    public static class EnumAlias
    {
        private static readonly Dictionary<Type, Dictionary<string, Enum>> enumMap = new();

        private static Dictionary<string, Enum> BuildMap<T>() where T : Enum
        {
            var map = new Dictionary<string, Enum>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                map[e.ToString().ToLower()] = (Enum)e;

                var member = typeof(T).GetMember(e.ToString());
                EnumAliasAttribute attr;
                if (member.Length > 0 && (attr = member[0].GetCustomAttribute<EnumAliasAttribute>()) != null)
                    foreach (var item in attr.Alias)
                        if (!map.TryAdd(item, (Enum)e))
                            throw new DuplicatedAliasException(item, map[item], (Enum)e);
            }
            enumMap[typeof(T)] = map;
            return map;
        }

        public static bool TryGet<T>(string name, out T value) where T : Enum
        {
            if (!enumMap.TryGetValue(typeof(T), out Dictionary<string, Enum> map))
                map = BuildMap<T>();

            if (map.TryGetValue(name, out var val))
            {
                value = (T)val;
                return true;
            }
            value = default;
            return false;
        }

        public static string[] GetAllAlias<T>() where T : Enum
        {
            if (!enumMap.TryGetValue(typeof(T), out Dictionary<string, Enum> map))
                map = BuildMap<T>();
            return map.Keys.ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EnumAliasAttribute : Attribute
    {
        public readonly string[] Alias;

        public EnumAliasAttribute(params string[] alias)
        {
            Alias = alias;
        }
    }
}