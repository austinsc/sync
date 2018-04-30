using System;
using System.Drawing;
using System.Threading.Tasks;
using Colorful;
using Console = Colorful.Console;

namespace CLI
{
    internal static class ConsoleLogger
    {
        public static void Status(string line) => Console.WriteLineFormatted("[{0}] -   {1}", Color.White, new Formatter(nameof(Status), Color.DeepSkyBlue), new Formatter(line, Color.LightGray));

        public static void Status(string line, Action task)
        {
            Console.WriteFormatted("[{0}] -   {1}", Color.White, new Formatter(nameof(Status), Color.DeepSkyBlue), new Formatter(line, Color.LightGray));
            task?.Invoke();
            Console.WriteLine("Done!", Color.LawnGreen);
        }

        public static async Task<T> Status<T>(string line, Func<Task<T>> task)
        {
            Console.WriteFormatted("[{0}] -   {1}", Color.White, new Formatter(nameof(Status), Color.DeepSkyBlue), new Formatter(line, Color.LightGray));
            var result = await task?.Invoke();
            Console.WriteLine("Done!", Color.LawnGreen);
            return result;
        }

        public static async Task Status(string line, Func<Task> task)
        {
            Console.WriteFormatted("[{0}] -   {1}", Color.White, new Formatter(nameof(Status), Color.DeepSkyBlue), new Formatter(line, Color.LightGray));
            await task();
            Console.WriteLine("Done!", Color.LawnGreen);
        }

        public static void Warning(string line) => Console.WriteLineFormatted("[{0}] -  {1}", Color.White, new Formatter(nameof(Warning), Color.Yellow), new Formatter(line, Color.LightGray));
        public static void Error(string line) => Console.WriteLineFormatted("[{0}] -    {1}", Color.White, new Formatter(nameof(Error), Color.DeepPink), new Formatter(line, Color.LightGray));
        public static void Debug(string line) => Console.WriteLineFormatted("[{0}] -    {1}", Color.White, new Formatter(nameof(Debug), Color.LawnGreen), new Formatter(line, Color.LightGray));
    }
}