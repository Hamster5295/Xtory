using System;
using System.Collections.Generic;

namespace Xtory
{
    public struct Arguments
    {
        private readonly string[] args;
        private int index;

        public Arguments(string[] args)
        {
            index = 0;
            this.args = args;
        }

        public readonly bool HasNext() => args.Length > index;

        private (string, int) Next()
        {
            if (!HasNext()) 
                throw new InstructionParseException(index, $"参数数量不足，应至少为 {index + 1} 个，实际为 {args.Length} 个");
            return (args[index], index++);
        }

        public readonly bool TryPeek(out string result)
        {
            if (!HasNext())
            {
                result = null;
                return false;
            }

            result = args[index];
            return true;
        }

        public string Raw() => args[Next().Item2];

        public int Int()
        {
            var (arg, index) = Next();

            if (!int.TryParse(arg, out var result))
                throw new InstructionParseException(index, $"参数应为整数，实际为非整数字符串 {args[index]}");

            return result;
        }

        public int IntOr(int value)
        {
            var (arg, _) = Next();
            return int.TryParse(arg, out var result) ? result : value;
        }

        private static readonly HashSet<string> trueAlias = new() { "true", "1", "t", "yes", "y", "是" };
        private static readonly HashSet<string> falseAlias = new() { "false", "0", "f", "no", "n", "否", "不是" };
        public bool Bool()
        {
            var (arg, index) = Next();
            arg = arg.Trim().ToLower();

            if (!trueAlias.Contains(arg) && !falseAlias.Contains(arg))
                throw new InstructionParseException(index, $"参数应为布尔值，实际为非布尔字符串 {args[index]}");

            return !falseAlias.Contains(arg);
        }

        public bool BoolOr(bool value)
        {
            var (arg, _) = Next();
            arg = arg.Trim().ToLower();

            if (!trueAlias.Contains(arg) && !falseAlias.Contains(arg)) return value;
            return !falseAlias.Contains(arg);
        }

        public float Float()
        {
            var (arg, index) = Next();

            if (!float.TryParse(arg, out var result))
                throw new InstructionParseException(index, $"参数应为浮点小数，实际为非浮点小数字符串 {args[index]}");
            return result;
        }

        public float FloatOr(float value)
        {
            var (arg, _) = Next();

            return float.TryParse(arg, out var result) ? result : value;
        }

        public string Str()
        {
            var (arg, index) = Next();

            if (arg.Length == 0)
                throw new InstructionParseException(index, $"参数应为字符串，实际无任何内容");
            return arg;
        }

        public string StrOr(string value)
        {
            var (arg, _) = Next();
            return arg.Length == 0 ? value : arg;
        }

        public string StrOrEmpty() => StrOr(string.Empty);

        public T Enum<T>() where T : Enum
        {
            var (arg, index) = Next();

            if (!EnumAlias.TryGet<T>(arg.Trim(), out var result))
                throw new InstructionParseException(index, $"参数应为 {string.Join(", ", EnumAlias.GetAllAlias<T>())} 中的某一个，实际为 {arg}");
            return result;
        }

        public T EnumOr<T>(T value) where T : Enum
        {
            var (arg, _) = Next();
            return EnumAlias.TryGet<T>(arg.Trim(), out var result) ? result : value;
        }
    }
}