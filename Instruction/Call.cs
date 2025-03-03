namespace Xtory.Instructions
{
    [Instruction("call", ExecuteMode.Blocking)]
    public class Call : Instruction
    {
        private readonly Location location;

        public Call(Arguments args) : base(args)
        {
            location = args.Loc();
        }

        public override void Execute(Runner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            handle.Call(location);
            handle.Complete();
        }
    }
}