using System;

namespace Xtory
{
    public interface IDataProvider
    {
        int GetInt(string key);
        float GetFloat(string key);
        bool GetBool(string key);
        string GetStr(string key);

        object Get(string key);
        void Set(string key, object value);

        string Format(string raw);
    }

    public abstract class DataProvider : IDataProvider
    {
        public abstract object Get(string key);
        public abstract void Set(string key, object value);

        public int GetInt(string key)
        {
            var val = Get(key);
            if (val is not int i) throw new InvalidCastException($"尝试将变量 {key} 读取为整数, 但其为 {val.GetType()}，当前值为 {val}");
            return i;
        }

        public float GetFloat(string key)
        {
            var val = Get(key);
            if (val is float f) return f;
            if (val is int i) return i;
            throw new InvalidCastException($"尝试将变量 {key} 读取为浮点数, 但其为 {val.GetType()}，当前值为 {val}");
        }

        public bool GetBool(string key)
        {
            var val = Get(key);
            if (val is bool b) return b;
            throw new InvalidCastException($"尝试将变量 {key} 读取为布尔值, 但其为 {val.GetType()}，当前值为 {val}");
        }

        public string GetStr(string key) => Get(key).ToString();

        public string Format(string raw)
        {
            FormatState state = FormatState.Idle;
            var builder = new System.Text.StringBuilder();
            var arg = new System.Text.StringBuilder();

            foreach (var c in raw)
                switch (state)
                {
                    case FormatState.Idle:
                        if (c == '$') state = FormatState.Begin;
                        else builder.Append(c);
                        break;

                    case FormatState.Begin:
                        if (c == '{')
                        {
                            state = FormatState.Key;
                            arg.Clear();
                        }
                        else
                        {
                            state = FormatState.Idle;
                            builder.Append('$');
                            builder.Append(c);
                        }
                        break;

                    case FormatState.Key:
                        if (c == '}')
                        {
                            state = FormatState.Idle;
                            builder.Append(GetStr(arg.ToString()));
                        }
                        else arg.Append(c);
                        break;
                }
            return builder.ToString();
        }

        private enum FormatState
        {
            Idle,
            Begin,
            Key,
        }
    }
}