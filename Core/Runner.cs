using System;
using System.Collections.Generic;

namespace Xtory
{
    public class Runner
    {
        public event Action<string> Ended;

        public bool Running { get; set; } = false;

        private readonly IDialogProvider dialog;
        private readonly IDataProvider data;
        private readonly IFileProvider file;

        private Story story;
        private int line;
        private long operationId;

        private readonly HashSet<long> pendingOps;
        private bool blocked = false;
        private readonly Stack<(Story, int)> callbackStack;

        public Runner(IDialogProvider dialog, IDataProvider data, IFileProvider file)
        {
            this.dialog = dialog;
            this.data = data;
            this.file = file;

            pendingOps = new();
            callbackStack = new();
        }

        public void Run(string file, string tag)
            => Run(this.file.Get(file), tag);

        public void Run(Story story, string tag)
        {
            if (!story.TryGetLine(tag, out var line)) throw new InvalidTagException(story, tag);
            Run(story, line);
        }

        public void Run(string file) => Run(this.file.Get(file), 0);
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
            if (!story.TryGetInst(line, out var inst, out var mode)) Stop();
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
                throw new InstructionExecException(currentLine, $"执行第 {currentLine + 1} 个指令时出错: \n{e.Message}\n{e.StackTrace}");
            }
        }

        public void Stop() => Stop(string.Empty);
        private void Stop(string ret)
        {
            Running = false;
            Ended?.Invoke(ret);
        }

        private void Seek(string file, string tag) => Seek(this.file.Get(file), tag);
        private void Seek(Story story, string tag)
        {
            this.story = story;
            Seek(tag);
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

        private void Call(string file, string tag)
        {
            Push();
            Seek(file, tag);
        }

        private void Call(string tag)
        {
            Push();
            Seek(tag);
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
            private readonly Runner runner;
            private readonly long opId;

            public Handle(Runner runner, long opId)
            {
                this.runner = runner;
                this.opId = opId;
            }

            public void Jump(string tag) => runner.Seek(tag);
            public void Jump(string file, string tag) => runner.Seek(file, tag);
            public void Jump(Location location)
            {
                if (location.file == null) runner.Seek(location.tag);
                else runner.Seek(location.file, location.tag);
            }

            public void Call(string tag) => runner.Call(tag);
            public void Call(string file, string tag) => runner.Call(file, tag);
            public void Call(Location location)
            {
                if (location.file == null) runner.Call(location.tag);
                else runner.Call(location.file, location.tag);
            }

            public void Return() => runner.Pop();
            public void Stop() => runner.Stop();
            public void Stop(string ret) => runner.Stop(ret);
            public void Complete() => runner.CompleteOperation(opId);
        }
    }

    public readonly struct Location
    {
        public const char SEP = ':';

        internal readonly string file, tag;

        public Location(string tag)
        {
            file = null;
            this.tag = tag;
        }

        public Location(string file, string tag)
        {
            this.file = file;
            this.tag = tag;
        }
    }
}