using System.Collections.Generic;

namespace Xtory
{
    public class Story
    {
        public readonly string Name;

        private readonly List<(Instruction, ExecuteMode)> insts;
        private readonly Dictionary<string, int> tagMap;

        public Story(string name, string content)
        {
            Name = name;

            Parser.Parse(content, out insts, out tagMap, out var diags);

            if (diags.Count > 0)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"解析文件 {name} 时出现以下问题:");
                foreach (var diagnosis in diags) sb.AppendLine(diagnosis.ToString());
                throw new XtoryParseException(sb.ToString());
            }
        }

        internal bool TryGetInst(string tag, out Instruction inst, out ExecuteMode mode)
        {
            if (tagMap.TryGetValue(tag, out var line)) return TryGetInst(line, out inst, out mode);
            inst = null;
            mode = ExecuteMode.Blocking;
            return false;
        }

        internal bool TryGetInst(int line, out Instruction inst, out ExecuteMode mode)
        {
            if (Has(line))
            {
                var elem = insts[line];
                inst = elem.Item1;
                mode = elem.Item2;
                return true;
            }
            inst = null;
            mode = ExecuteMode.Blocking;
            return false;
        }

        internal bool TryGetLine(string tag, out int line)
            => tagMap.TryGetValue(tag, out line);

        internal bool Has(string tag) => tagMap.ContainsKey(tag);
        internal bool Has(int line) => line < insts.Count;
    }
}