using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xtory
{
    internal static class Parser
    {
        private static readonly Dictionary<string, Type> instMap;
        private static readonly Dictionary<string, ExecuteMode> modeMap;
        private static readonly Dictionary<string, int> priorityMap;

        static Parser()
        {
            instMap = new();
            modeMap = new();
            priorityMap = new();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(i => i.GetTypes())
                                    .Where(i => !i.IsAbstract && i.IsSubclassOf(typeof(Instruction))))
            {
                InstructionAttribute attr;
                if ((attr = type.GetCustomAttribute<InstructionAttribute>()) != null)
                {
                    var header = attr.Header.ToLower();
                    if (!instMap.TryAdd(header, type))
                    {
                        if (attr.Priority > priorityMap[header])
                        {
                            instMap[header] = type;
                            modeMap[header] = attr.Mode;
                            priorityMap[header] = attr.Priority;
                        }
                        else if (attr.Priority == priorityMap[header])
                            throw new DuplicatedHeaderException(header, instMap[header], type, attr.Priority);
                    }
                    else
                    {
                        modeMap.Add(header, attr.Mode);
                        priorityMap.Add(header, attr.Priority);
                    }

                }
            }
        }

        internal static void Parse(string text, out List<(Instruction, ExecuteMode)> insts, out Dictionary<string, int> tagMap, out List<Diagnosis> diags)
        {
            insts = new();
            tagMap = new();
            diags = new();

            var lineIdx = 0;
            var line = new List<string>();
            var buf = new StringBuilder(256);

            var state = ParseState.Idle;

            foreach (var c in text)
            {
                switch (state)
                {
                    case ParseState.Idle:
                        switch (c)
                        {
                            case '"':
                                state = ParseState.Str;
                                break;

                            case ',':
                                line.Add(buf.ToString());
                                buf.Clear();
                                break;

                            case '\n':
                                var currentLine = lineIdx++;

                                line.Add(buf.ToString());
                                var args = line.ToArray();
                                line.Clear();
                                buf.Clear();

                                if (args.Length < 4) continue;
                                if (args.Select(i => i.Trim()).All(i => i.Length == 0)) continue;

                                // 第一列，有内容则禁用
                                if (args[0].Trim().Length > 0) continue;

                                // 第二列，执行模式
                                var modeTxt = args[1].Trim().ToLower();

                                // 第三列建立 Tag 表
                                var tag = args[2].Trim();
                                if (tag.Length > 0)
                                    if (!tagMap.TryAdd(tag, insts.Count))
                                        diags.Add(new(currentLine, 2, $"标签 {tag} 已存在，请检查标签一列！"));
                                

                                // 第四列，指令
                                if (TryParseInstruction(modeTxt, args[3..], out var inst, out var mode, out var diagnosis))
                                    insts.Add((inst, mode));
                                else
                                {
                                    diagnosis.line = currentLine;
                                    diags.Add(diagnosis);
                                }
                                break;

                            default:
                                buf.Append(c);
                                break;
                        }
                        break;

                    case ParseState.Str:
                        if (c == '"') state = ParseState.Quote;
                        else buf.Append(c);
                        break;

                    case ParseState.Quote:
                        if (c == '"') { buf.Append(c); state = ParseState.Str; }
                        else state = ParseState.Idle;
                        break;
                }
            }
        }

        internal static bool TryParseInstruction(string modeStr, string[] args, out Instruction inst, out ExecuteMode mode, out Diagnosis diagnosis)
        {
            inst = null;
            diagnosis = default;
            mode = ExecuteMode.Blocking;

            var header = args[0].Trim().ToLower();
            var instArgs = args[1..];

            // 指令类型
            if (!instMap.TryGetValue(header, out var type))
            {
                diagnosis = new(3, $"未知的指令 {header}");
                return false;
            }

            // 执行模式
            if (modeStr.Length == 0) mode = modeMap[header];
            else if (!EnumAlias.TryGet(modeStr, out mode))
            {
                diagnosis = new(2, $"未知的执行模式 {mode}");
                return false;
            }

            // 解析参数
            try
            {
                inst = Activator.CreateInstance(type, new Arguments(instArgs)) as Instruction;
            }
            catch (TargetInvocationException err)
            {
                if (err.InnerException is InstructionParseException e)
                {
                    diagnosis = e.Diagnosis;
                    diagnosis.message = $"在解析 {header} 时，{diagnosis.message}";
                    diagnosis.column += 4;
                }
                return false;
            }

            return true;
        }

        private enum ParseState
        {
            Idle,
            Quote,
            Str             // 纯文本
        }
    }


    internal struct Diagnosis
    {
        internal int line, column;
        internal string message;

        internal Diagnosis(int column, string message)
        {
            line = 0;
            this.column = column;
            this.message = message;
        }

        internal Diagnosis(int line, int column, string message)
        {
            this.line = line;
            this.column = column;
            this.message = message;
        }

        public readonly override string ToString()
            => $"第 {line + 1} 行, 第 {column + 1} 列出错: {message}";
    }
}