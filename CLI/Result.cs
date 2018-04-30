using System.Collections.Generic;
using System.Linq;

namespace CLI
{
    public class Result<T, TKey> 
        where T: ISyncable<TKey>, new()
    {
        public Result(IEnumerable<T> data)
        {
            this.Data = data.GroupBy(x => x.SyncKey).Select(x => x.First()).ToList();
            this.HashMap = this.Data.ToDictionary(x => x.SyncKey, x => x.SyncHash);
            this.Hash = string.Join(string.Empty, this.Data.OrderBy(x => x.SyncKey).Select(x => x.SyncHash)).ComputeHash();
        }

        public List<T> Data { get; }
        public Dictionary<TKey, string> HashMap { get; }
        public string Hash { get; }
    }
}