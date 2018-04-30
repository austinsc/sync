using Bogus;

namespace CLI
{
    public class SyncablePerson : Person, ISyncable<string>
    {
        public string SyncKey => this.Email;
        public string SyncHash => this.ComputeHash();
    }
}