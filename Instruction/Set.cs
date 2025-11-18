namespace Xtory.Instructions
{
    [Instruction("set", ExecuteMode.Blocking)]
    public class Set : Instruction
    {
        private readonly string key, value;

        public Set(Arguments args) : base(args)
        {
            key = args.Str();
            value = args.StrOrEmpty();
        }

        public override void Execute(XtoryRunner.Handle handle, IInterfaceProvider dialog, IDataProvider data)
        {
            data.Set(key, value);
            handle.Complete();
        }
    }
}