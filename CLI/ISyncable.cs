using Newtonsoft.Json;

namespace CLI
{
    public interface ISyncable<out T>
    {
        T SyncKey { get; }
        [JsonIgnore]
        string SyncHash { get; }
    }
}