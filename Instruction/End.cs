namespace Xtory.Instructions
{
    [Instruction("end", ExecuteMode.Blocking)]
    public class End : Instruction
    {
        private readonly string ret;

        public End(Arguments args) : base(args)
        {
            ret = args.StrOrEmpty();
        }

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            handle.Stop(ret);
        }
    }
}