using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericsExample.Generics;
using System.Diagnostics;

namespace GenericsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RemoveWhiteSpace();

                //ProcessHelper.RedirectNetCommandStreams();
                //ProcessHelper.SortInputListText();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(e.ToString());
            }
            //BasicGenerics.NonGenericPerfromance();
            //BasicGenerics.GenericPerfromance();
            //CloseWindow();
        }

        private static void RemoveWhiteSpace()
        {
            string str1 = "\n          \n          Welcome to the Freemake Video Downloader Setup Wizard";
            string str2 = "Help make Freemake software better by automatically sending anonymous usage statistics\nand crash report to Freenote.";
            string str3 = "\n          \n          Welcome to the Freemake Video\n          \n          Downloader Setup Wizard            \n        \n        ";

            Console.WriteLine(str1.Trim());
            Console.WriteLine(str2.Trim());

            RemoveExtraWhiteSpace(str3.Trim(), " ".ToCharArray());

            Console.ReadLine();
        }

        public static string RemoveDuplicateChars(string src, char[] dupes)
        {
            var sd = (char[])dupes.Clone();
            Array.Sort(sd);

            var res = new StringBuilder(src.Length);

            for (int i = 0; i < src.Length; i++)
            {
                if (i == 0 || src[i] != src[i - 1] || Array.BinarySearch(sd, src[i]) < 0)
                {
                    res.Append(src[i]);
                }
            }
            return res.ToString();
        }

        public static string RemoveExtraWhiteSpace(string src, char[] wsChars)
        {
            //this line removes whitespace inbetween the string.
            return string.Join(" ", src.Split(wsChars, StringSplitOptions.RemoveEmptyEntries));
        }

        private static void CloseWindow()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }
    }
}