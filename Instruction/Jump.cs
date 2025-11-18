namespace Xtory.Instructions
{
    [Instruction("jump", ExecuteMode.Blocking)]
    public class Jump : Instruction
    {
        private readonly string location;

        public Jump(Arguments args) : base(args)
        {
            location = args.Str();
        }

        public override void Execute(XtoryRunner.Handle handle, IInterfaceProvider dialog, IDataProvider data)
        {
            handle.Jump(location);
            handle.Complete();
        }
    }
}