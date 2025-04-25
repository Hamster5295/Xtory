using System;

namespace Xtory
{
    public readonly struct Condition
    {
        private readonly string opA, opB;
        private readonly bool isVarA, isVarB;
        private readonly Comparer cmp;

        public Condition(string a, Comparer cmp, string b)
        {
            this.cmp = cmp;

            var at = a.Trim();
            if (at.StartsWith("${") && at.EndsWith("}"))
            {
                opA = at[2..^1];
                isVarA = true;
            }
            else
            {
                opA = a;
                isVarA = false;
            }

            var bt = b.Trim();
            if (bt.StartsWith("${") && bt.EndsWith("}"))
            {
                opB = at[2..^1];
                isVarB = true;
            }
            else
            {
                opB = b;
                isVarB = false;
            }
        }

        public bool Eval(IDataProvider data)
        {
            DataType ta, tb;
            object a, b;

            if (isVarA)
            {
                a = data.Get(opA);
                ta = DataUtils.GetObjectType(a);
            }
            else ta = DataUtils.CastString(opA, out a);

            if (isVarB)
            {
                b = data.Get(opB);
                tb = DataUtils.GetObjectType(b);
            }
            else tb = DataUtils.CastString(opB, out b);

            if (ta == DataType.Other) throw new InvalidCastException($"变量 {opA} 的类型为 {ta}，应为 Int, Float, Bool, String 中的一个");
            if (tb == DataType.Other) throw new InvalidCastException($"变量 {opB} 的类型为 {tb}，应为 Int, Float, Bool, String 中的一个");

            if (!DataUtils.GetCompareType(ta, tb, out var t)) throw new InvalidOperationException($"无法将类型为 {ta} 的变量 {opA} 与 类型为 {tb} 的变量 {opB} 进行比较");
            if (cmp != Comparer.Equal && cmp != Comparer.Unequal && t != DataType.Int && t != DataType.Float)
                throw new InvalidOperationException($"类型为 {t} 的比较式的变量无法比较大小，仅能判断等于/不等于");

            return cmp switch
            {
                Comparer.Equal => a.Equals(b),
                Comparer.Unequal => !a.Equals(b),
                Comparer.Less => (a as IComparable).CompareTo(b) < 0,
                Comparer.LessEqual => (a as IComparable).CompareTo(b) <= 0,
                Comparer.Greater => (a as IComparable).CompareTo(b) > 0,
                Comparer.GreaterEqual => (a as IComparable).CompareTo(b) >= 0,
                _ => throw new InvalidOperationException($"未知的关系符号 {cmp}"),
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