namespace Xtory.Instructions
{
    [Instruction("jump", ExecuteMode.Blocking)]
    public class Jump : Instruction
    {
        private readonly Location location;

        public Jump(Arguments args) : base(args)
        {
            location = args.Loc();
        }

        public override void Execute(Runner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            handle.Jump(location);
            handle.Complete();
        }
    }
}