namespace Xtory.Instructions
{
    [Instruction("ret", ExecuteMode.Blocking)]
    public class Return : Instruction
    {
        public Return(Arguments args) : base(args) { }

        public override void Execute(Runner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            handle.Return();
            handle.Complete();
        }
    }
}