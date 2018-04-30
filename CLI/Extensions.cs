using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace CLI
{
    public static class Extensions
    {
        private static readonly HashAlgorithm _algorithm = new Fnv1a32();

        public static string ComputeHash(this string @this) => string.Join(string.Empty, _algorithm.ComputeHash(Encoding.UTF8.GetBytes(@this)).Select(x => x.ToString("X2")));

        public static string ComputeHash<T>(this ISyncable<T> @this) => JsonConvert.SerializeObject(@this, Formatting.None).ComputeHash();
    }
}