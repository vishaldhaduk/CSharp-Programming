using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace MozHelper
{
    public static class IOHelper
    {
        private static readonly Regex Delimeter = new Regex(@"[\\]+", RegexOptions.Compiled);

        public static string Join2(this string value, params string[] args)
        {
            return value.ToFile(args);
        }
        public static string ToFile(this string value, params string[] args)
        {
            return new[] { value }
                .Concat(args)
                .ToArray()
                .ToFile();
        }

        public static string ToFile(this IEnumerable<string> args)
        {
            return Delimeter.Replace(string.Join(@"\", args.ToArray()), @"\");
        }
    }
}
