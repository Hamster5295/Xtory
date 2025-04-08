using System;

namespace Xtory
{
    public class InvalidTagException : Exception
    {
        public InvalidTagException(Story story, string message) : base($"文件 {story.Name} 中不存在 '{message}' 标签") { }
    }
}