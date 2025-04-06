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

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            DataUtils.CastString(value, out var result);
            data.Set(key, result);
            handle.Complete();
        }
    }
}