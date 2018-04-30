using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CLI.API;
using Console = Colorful.Console;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        private static async Task MainAsync(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var current = await ConsoleLogger.Status($"Generating {Settings.RecordCount} records... ", RetrieveData.GetPeople);
            ConsoleLogger.Debug($"Elapsed Time: {sw.ElapsedMilliseconds} ms.");
            ConsoleLogger.Status($"Hash:\t{current.Hash}... ", () => {});

            while(true)
            {
                sw.Restart();
                var result = await ConsoleLogger.Status($"Generating {Settings.RecordCount} records... ", RetrieveData.GetPeople);
                ConsoleLogger.Debug($"Elapsed Time: {sw.ElapsedMilliseconds} ms.");
                ConsoleLogger.Status($"Hash:\t{result.Hash}... ", () => {});
                if(string.Compare(result.Hash, current.Hash, StringComparison.InvariantCulture) != 0)
                {
                    ConsoleLogger.Debug("Changes detected on at least one entity... ");
                    var keys = new[] { current.HashMap, result.HashMap }.SelectMany(x => x.Keys).Distinct().OrderBy(x => x).ToList();
                    var zipped = keys.Select(x => new { Key = x, Current = current.HashMap.ContainsKey(x) ? current.HashMap[x] : string.Empty, New = result.HashMap.ContainsKey(x) ? result.HashMap[x] : string.Empty});
                    ConsoleTable.From(zipped, "Key", "Current", "New").Write();
                    current = result;
                }
                else
                    ConsoleLogger.Debug("No changes detected.");

                if(Console.ReadKey().Key == ConsoleKey.Enter)
                    break;
            }
            //ConsoleTable.From(result.Data, "SyncKey", "SyncHash", "FirstName", "LastName").Write();
        }
    }
}
