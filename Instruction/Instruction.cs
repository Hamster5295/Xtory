using System;

namespace Xtory
{
    public abstract class Instruction
    {
        public Instruction(Arguments args) { }
        public abstract void Execute(XtoryRunner.Handle handle, IInterfaceProvider dialog, IDataProvider data);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InstructionAttribute : Attribute
    {
        internal readonly string Header;
        internal readonly ExecuteMode Mode;
        internal readonly int Priority;

        public InstructionAttribute(string header, ExecuteMode mode, int priority = 0)
        {
            Header = header;
            Mode = mode;
            Priority = priority;
        }
    }

    public enum ExecuteMode
    {
        [EnumAlias("by", "pass", "自由", "旁路")]
        Bypass,

        [EnumAlias("p", "para", "并行")]
        Parallel,

        [EnumAlias("b", "block", "等待", "阻塞")]
        Blocking,
    }
}