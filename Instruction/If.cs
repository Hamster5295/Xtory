using System;
using System.Collections.Generic;

namespace Xtory.Instructions
{
    [Instruction("if", ExecuteMode.Blocking)]
    public class If : Instruction
    {
        private readonly List<HashSet<Condition>> conditions;
        private readonly Location target;

        public If(Arguments args) : base(args)
        {
            conditions = new();
            int idx = 0;

            target = args.Loc();

            do
            {
                var a = args.Str();
                var cmp = args.Enum<Comparer>();
                var b = args.Str();
                if (conditions.Count <= idx) conditions.Add(new());
                conditions[idx].Add(new(a, cmp, b));

                if (args.TryPeek(out var value) && value.Trim().Length > 0)
                {
                    if (args.Enum<Combiner>() == Combiner.Or) idx++;
                }
                else break;

            } while (true);
        }

        public override void Execute(Runner.Handle handle, IDialogProvider dialog, IDataProvider data)
        {
            var cond = false;
            foreach (var orBlock in conditions)
            {
                var andCond = true;
                foreach (var andBlock in orBlock)
                    andCond &= andBlock.Eval(data);
                cond |= andCond;
            }
            if (cond) handle.Jump(target);
            handle.Complete();
        }
    }
}