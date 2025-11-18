namespace Xtory.Instructions
{
    [Instruction("clear", ExecuteMode.Blocking)]
    public class Clear : Instruction
    {
        public Clear(Arguments args) : base(args) { }

        public override void Execute(XtoryRunner.Handle handle, IInterfaceProvider dialog, IDataProvider data)
        {
            dialog.ShowText(string.Empty);
            dialog.ClearMenu(s => handle.Complete());
        }
    }
}