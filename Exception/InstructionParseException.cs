using System;

namespace Xtory
{
    public class InstructionParseException : Exception
    {
        internal readonly Diagnosis Diagnosis;

        public InstructionParseException(int col, string message)
            : base(message)
        {
            Diagnosis = new(col, message);
        }
    }
}