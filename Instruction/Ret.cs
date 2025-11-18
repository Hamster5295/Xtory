namespace Xtory.Instructions
{
    [Instruction("ret", ExecuteMode.Blocking)]
    public class Return : Instruction
    {
        public Return(Arguments args) : base(args) { }

        public override void Execute(XtoryRunner.Handle handle, IInterfaceProvider dialog, IDataProvider data)
        {
            handle.Return();
            handle.Complete();
        }
    }
}