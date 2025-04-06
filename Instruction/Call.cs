namespace Xtory.Instructions
{
    [Instruction("call", ExecuteMode.Blocking)]
    public class Call : Instruction
    {
        private readonly string location;

        public Call(Arguments args) : base(args)
        {
            location = args.Str();
        }

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            handle.Call(location);
            handle.Complete();
        }
    }
}