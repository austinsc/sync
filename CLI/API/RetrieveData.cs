using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLI.API
{
    public static class RetrieveData
    {
        private static readonly Random _random = new Random();
        private static readonly Lazy<List<SyncablePerson>> _originalPeople = new Lazy<List<SyncablePerson>>(GeneratePeople);
        private static bool CoinToss => _random.Next(0, 2) == 0;

        private static List<SyncablePerson> GeneratePeople() => Enumerable.Range(0, Settings.RecordCount).Select(x => new SyncablePerson()).OrderBy(x => x.SyncKey).ToList();

        public static Task<Result<SyncablePerson, string>> GetPeople() => Task.FromResult(new Result<SyncablePerson, string>(CoinToss
                                                                                                                                 ? _originalPeople.Value
                                                                                                                                 : _originalPeople.Value
                                                                                                                                                  .Skip(_random.Next(0, Settings.RecordCount / 2 - 1))
                                                                                                                                                  .Take(_random.Next(0, Settings.RecordCount / 2 + 1))
                                                                                                                                                  .Concat(GeneratePeople().Take(_random.Next(0, Settings.RecordCount)))));
    }
}