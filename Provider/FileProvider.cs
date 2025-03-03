using System.Collections.Generic;

namespace Xtory
{
    public interface IFileProvider
    {
        Story Get(string file);
    }

    public abstract class FileProvider
    {
        private readonly Dictionary<string, Story> cache = new();

        public Story Get(string file)
        {
            if (cache.TryGetValue(file, out Story t)) return t;
            return cache[file] = new Story(file, Load(file));
        }

        public abstract string Load(string file);
    }
}