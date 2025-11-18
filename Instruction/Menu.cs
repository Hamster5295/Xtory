namespace Xtory.Instructions
{
    [Instruction("menu", ExecuteMode.Bypass)]
    public class Menu : Instruction
    {
        private readonly string tag, content;

        public Menu(Arguments args) : base(args)
        {
            content = args.Str();
            tag = args.Str();
        }

        public override void Execute(XtoryRunner.Handle handle, IInterfaceProvider dialog, IDataProvider data)
        {
            dialog.ShowMenu(data.Format(content), s =>
                {
                    handle.Jump(tag);
                    dialog.ClearMenu(
                        s => handle.Complete()
                    );
                });
        }
    }
}