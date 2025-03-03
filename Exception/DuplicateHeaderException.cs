using System;

namespace Xtory
{
    public class DuplicatedHeaderException : Exception
    {
        public DuplicatedHeaderException(string header, Type t1, Type t2, int priority)
            : base($"指令 '{header}' 不应当被 {t1} 和 {t2} 两个类型同时解析，二者提供了相同的优先级 {priority}") { }
    }
}