using System;

namespace Xtory
{
    public class InstructionExecException : Exception
    {
        public InstructionExecException(int line, string message) : base(message) { }
    }
}