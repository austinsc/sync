using System.Collections.Generic;

namespace CLI
{
    public class ConsoleTableOptions
    {
        public IEnumerable<string> Columns { get; set; } = new List<string>();
        public bool EnableCount { get; set; } = true;
    }
}