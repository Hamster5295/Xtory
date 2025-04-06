using System.Collections.Generic;

namespace Xtory.Instructions
{
    [Instruction("menu", ExecuteMode.Blocking)]
    public class Menu : Instruction
    {
        private readonly Dictionary<string, string> map;

        public Menu(Arguments args) : base(args)
        {
            map = new();

            while (args.HasNext() && args.TryPeek(out var value) && value.Trim().Length > 0)
                map.Add(args.Str(), args.Str());
        }

        public override void Execute(XtoryRunner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            foreach (var item in map)
                dialog.ShowMenu(data.Format(item.Key), s =>
                    {
                        handle.Jump(item.Value);
                        dialog.ClearMenu(
                            s => handle.Complete()
                        );
                    });
        }
    }
}