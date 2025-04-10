namespace Xtory.Instructions
{
    [Instruction("halt", ExecuteMode.Bypass)]
    public class Halt : Instruction
    {
        public Halt(Arguments args) : base(args) { }

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            handle.Halt();
        }
    }
}