using System;

namespace Xtory
{
    public interface IDataProvider
    {
        string Get(string key);
        void Set(string key, string value);

        string Format(string raw);
    }

    public abstract class DataProvider : IDataProvider
    {
        public abstract string Get(string key);
        public abstract void Set(string key, string value);

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
                            builder.Append(Get(arg.ToString()));
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