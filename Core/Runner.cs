using System;
using System.Collections.Generic;

namespace Xtory
{
    public class XtoryRunner
    {
        public event Action<string> Ended;

        public bool Running { get; set; } = false;

        private readonly IInterfaceProvider dialog;
        private readonly IDataProvider data;

        private Story story;
        private int line;
        private long operationId;

        private readonly HashSet<long> pendingOps;
        private bool blocked = false;
        private readonly Stack<(Story, int)> callbackStack;

        public XtoryRunner(IInterfaceProvider dialog, IDataProvider data)
        {
            this.dialog = dialog;
            this.data = data;

            pendingOps = new();
            callbackStack = new();
        }

        public void Run(Story story, string tag)
        {
            if (tag == null || tag.Length == 0) Run(story);
            else if (story.TryGetLine(tag, out var l)) Run(story, l);
            else throw new InvalidTagException(story, tag);
        }

        public void Run(Story story) => Run(story, 0);

        public void Run(Story story, int line)
        {
            if (Running) return;

            this.story = story;

            Seek(line);
            operationId = 0;
            pendingOps.Clear();
            callbackStack.Clear();

            Running = true;

            Execute();
        }

        public void Tick()
        {
            if (!Running) return;
            if (blocked) return;
            Execute();
        }

        private void Execute()
        {
            if (!story.TryGetInst(line, out var inst, out var mode)) { Stop(); return; }
            var currentLine = line++;

            if (mode == ExecuteMode.Parallel)
                pendingOps.Add(operationId);

            if (mode == ExecuteMode.Blocking)
            {
                pendingOps.Add(operationId);
                blocked = true;
            }

            try
            {
                inst.Execute(new Handle(this, operationId++), dialog, data);
            }
            catch (Exception e)
            {
                throw new InstructionExecException(currentLine, $"执行第 {currentLine + 1} 个指令 {inst.GetType()} 时出错: \n{e.Message}\n{e.StackTrace}");
            }
        }

        public void Stop() => Stop(string.Empty);
        private void Stop(string ret)
        {
            Running = false;
            Ended?.Invoke(ret);
        }

        private void Seek(string tag)
        {
            if (!story.TryGetLine(tag, out var line)) throw new InvalidTagException(story, tag);
            Seek(line);
        }

        private void Seek(int line)
        {
            this.line = line;
        }

        private void Call(string tag)
        {
            Push();
            Seek(tag);
        }

        private void Halt()
        {
            line--;
        }

        private void Push() => callbackStack.Push((story, line));

        private void Pop()
        {
            var (story, line) = callbackStack.Pop();
            this.story = story;
            Seek(line);
        }

        private void CompleteOperation(long id)
        {
            pendingOps.Remove(id);
            if (blocked && pendingOps.Count == 0) blocked = false;
        }

        public readonly struct Handle
        {
            private readonly XtoryRunner runner;
            private readonly long opId;

            public Handle(XtoryRunner runner, long opId)
            {
                this.runner = runner;
                this.opId = opId;
            }

            public void Jump(string tag) => runner.Seek(tag);
            public void Call(string tag) => runner.Call(tag);
            public void Halt() => runner.Halt();

            public void Return() => runner.Pop();
            public void Stop() => runner.Stop();
            public void Stop(string ret) => runner.Stop(ret);
            public void Complete() => runner.CompleteOperation(opId);
        }
    }
}