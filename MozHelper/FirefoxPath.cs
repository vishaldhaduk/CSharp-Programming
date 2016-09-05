using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MozHelper
{
   public static class FirefoxPath
    {
        private static readonly string FireFox;
        static FirefoxPath()
        {
            FireFox = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Join2("Mozilla", "Firefox");
        }

        public static string GetPathToProfiles
        {
            get
            {
                return FireFox;
            }
        }

        public static string SearchMetadataFilePath
        {
            get
            {
                return GetProfileFolder().Join2("search-metadata.json");
            }
        }

        public static string SettingsFilePath
        {
            get
            {
                return GetProfileFolder().Join2("prefs.js");
            }
        }

        public static string SearchJsonFilePath
        {
            get
            {
                return GetProfileFolder().Join2("search.json");
            }
        }

        public static string SearchJsonlz4FilePath
        {
            get
            {
                return GetProfileFolder().Join2("search.json.mozlz4");
            }
        }

        public static string BackupSearchJsonFilePath
        {
            get
            {
                return GetProfileFolder().Join2("b_search.json");
            }
        }

        public static string UserSettingsFilePath
        {
            get
            {
                return GetProfileFolder().Join2("user.js");
            }
        }

        public static string CertDb
        {
            get
            {
                return GetProfileFolder().Join2("cert8.db");
            }
        }

        public static string AppDataProfile
        {
            get
            {
                return GetProfileFolder();
            }
        }

        public static string ExtensionsJsonFilePath
        {
            get
            {
                return GetProfileFolder().Join2("extensions.json");
            }
        }

        private static string PluginsPath
        {
            get
            {
                return GetProfileFolder().Join2("searchplugins");
            }
        }

        private static string GetPathToIndividualCodeFolder(string directory)
        {
            var result = string.Empty;

            if (File.Exists(directory))
            {
                foreach (var s in File.ReadAllLines(directory))
                {
                    if (s.StartsWith("Path="))
                    {
                        result = s;
                    }
                    else if (s.StartsWith("Default=1"))
                    {
                        break;
                    }
                }
            }
            return result.Replace("Path=", string.Empty).ToFile();
        }

        public static void DeleteAllPlugins()
        {
            // in FireFox 35 it doesn't exist
            if (Directory.Exists(PluginsPath))
            {
                Array.ForEach(Directory.GetFiles(PluginsPath), File.Delete);
            }
        }

        public static void DeleteSearchJSon()
        {
            if (File.Exists(SearchJsonFilePath))
                File.Delete(SearchJsonFilePath);

            if (File.Exists(SearchJsonlz4FilePath))
                File.Delete(SearchJsonlz4FilePath);
        }
        private static string GetProfileFolder()
        {
            var path = @"C:\Users\vishal.dhaduk.LAVASOFT\AppData\Roaming\Mozilla\Firefox\Profiles\9xca3ebe.default\";

            return path;
        }

        public static string GetFirefoxPath()
        {
            string ffInstallPath = Environment.GetEnvironmentVariable("ProgramFiles(x86)"); ;
            if (String.IsNullOrEmpty(ffInstallPath))
            {
                ffInstallPath = Environment.GetEnvironmentVariable("ProgramFiles");
            }

            return Path.Combine(ffInstallPath, "Mozilla Firefox");
        }
    }
}
