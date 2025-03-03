using System;

namespace Xtory
{
    public class DuplicatedAliasException : Exception
    {
        public DuplicatedAliasException(string alias, Enum t1, Enum t2)
            : base($"别名 '{alias}' 不应当被 {t1} 和 {t2} 两个枚举同时解析") { }
    }
}