using System;
using System.Diagnostics;

namespace Xtory
{
    public readonly struct Condition
    {
        private readonly string opA, opB;
        private readonly bool isVarA = false, isVarB = false;
        private readonly Comparer cmp;

        public Condition(string a, Comparer cmp, string b)
        {
            this.cmp = cmp;

            var at = a.Trim();
            if (at.StartsWith("${") && at.EndsWith("}")) { opA = at[2..^1]; isVarA = true; }
            else opA = a;

            var bt = b.Trim();
            if (bt.StartsWith("${") && bt.EndsWith("}")) { opB = at[2..^1]; isVarB = true; }
            else opB = b;
        }

        public bool Eval(IDataProvider data)
        {
            var isDigitA = float.TryParse(isVarA ? data.Get(opA) : opA, out var digitA);
            var isDigitB = float.TryParse(isVarB ? data.Get(opB) : opB, out var digitB);

            switch (cmp)
            {
                case Comparer.Equal: return opA == opB;
                case Comparer.Unequal: return opA != opB;
            }

            if (!isDigitA) throw new InvalidCastException($"左侧比较数 {opA} 不是数字，无法比较大小！");
            if (!isDigitB) throw new InvalidCastException($"右侧比较数 {opB} 不是数字，无法比较大小！");

            return cmp switch
            {
                Comparer.Greater => digitA > digitB,
                Comparer.GreaterEqual => digitA >= digitB,
                Comparer.Less => digitA < digitB,
                Comparer.LessEqual => digitA <= digitB,
                _ => throw new UnreachableException("前侧 switch 已经全部判断完毕，不可能执行此处代码"),
            };
        }
    }

    public enum Comparer
    {
        [EnumAlias("eq", "equals", "=", "==", "等于")]
        Equal,
        [EnumAlias("ne", "neq", "!=", "不等于")]
        Unequal,
        [EnumAlias("g", "gt", ">", "大于")]
        Greater,
        [EnumAlias("ge", ">=", "大于等于")]
        GreaterEqual,
        [EnumAlias("l", "lt", "<", "小于")]
        Less,
        [EnumAlias("le", "<=", "小于等于")]
        LessEqual,
    }

    public enum Combiner
    {
        [EnumAlias("a", "&", "&&", "且")]
        And,
        [EnumAlias("o", "|", "||", "或")]
        Or
    }
}