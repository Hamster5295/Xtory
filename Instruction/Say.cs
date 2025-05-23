namespace Xtory.Instructions
{
    [Instruction("say", ExecuteMode.Blocking)]
    public class Say : Instruction
    {
        private readonly string character, content;

        public Say(Arguments args) : base(args)
        {
            character = args.StrOrEmpty();
            content = args.Str();
        }

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            dialog.ShowCharacter(character);
            dialog.ShowText(data.Format(content), s => handle.Complete());
        }
    }

    [Instruction("show", ExecuteMode.Blocking)]
    public class Show : Instruction
    {
        private readonly string content;

        public Show(Arguments args) : base(args)
        {
            content = args.Str();
        }

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            dialog.ShowCharacter("");
            dialog.ShowText(data.Format(content), s => handle.Complete());
        }
    }
}