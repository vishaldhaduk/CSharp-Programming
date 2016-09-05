using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Lavasoft.SearchProtect.Business.Browsers.FireFox.Helpers
{
    public static class MetadataHelper
    {
        /// <summary>
        /// Replaces metadata file in FireFox profile folder. As of Firefox 40, browser reads machine current search engine from search metadata json file and validates the hash.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="defaultProvider"></param>
        public static void MetadataJsonReplace(string provider, string defaultProvider)
        {
            string Pattern = "\"current\":\"{0}\",\"hash\":\"{1}\", \"searchdefault\":\"{2}\",\"searchdefaultexpir\":1442063562305.0,\"searchdefaulthash\":\"{3}\"";
            string pathToProfiles = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles";

            //get profile directory

            string[] directories = Directory.GetDirectories(pathToProfiles);
            foreach (string directory in directories)
            {
                StringBuilder newPattern = new StringBuilder();
                string providerHash = ComputeMetadataHash(directory, provider);
                string defaultProviderHash = ComputeMetadataHash(directory, defaultProvider);
                newPattern.AppendFormat(Pattern, provider, providerHash, defaultProvider, defaultProviderHash);

                if (File.Exists(directory + @"\search-metadata.json"))
                {
                    File.Delete(directory + @"\search-metadata.json");
                }
                using (StreamWriter fs = new StreamWriter(directory + @"\search-metadata.json"))
                {
                    fs.Write("{\"[global]\":{" + newPattern.ToString() + "}}");
                }
            }
        }

        /// <summary>
        /// Method that computes the metadata hash SHA256 from -> (profile name + engine anme + disclaimer)
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string ComputeMetadataHash(string directory, string provider)
        {
            Encoding encoding = new UTF8Encoding();
            SHA256 hash = SHA256.Create();

            string Disclaimer = "By modifying this file, I agree that I am doing so only ";
            Disclaimer += "within Firefox itself, using official, user-driven search ";
            Disclaimer += "engine selection processes, and in a way which does not ";
            Disclaimer += "circumvent user consent. I acknowledge that any attempt ";
            Disclaimer += "to change this file from outside of Firefox is a malicious ";
            Disclaimer += "act, and will be responded to accordingly.";

            string dirName = new DirectoryInfo(directory).Name;
            byte[] encode = encoding.GetBytes(dirName + provider + Disclaimer);
            byte[] computedHash = hash.ComputeHash(encode);
            string hashString = System.Convert.ToBase64String(computedHash);

            return hashString;
        }

        public static string GetCurrentSearch()
        {
            try
            {
                string pathToProfiles = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles";

                //get profile directory
                string[] directories = Directory.GetDirectories(pathToProfiles);
                foreach (string directory in directories)
                {
                    return GetSearchNameFromMetadata(directory + @"\search-metadata.json");
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// If current engine is not set, it means that it is default engine
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        private static string GetSearchNameFromMetadata(string jsonPath)
        {
            string result = "";
            string defaultSearch = "Google";

            if (File.Exists(jsonPath))
            {
                try
                {
                    JObject o = JObject.Parse(File.ReadAllText(jsonPath));
                    var global = o["[global]"];
                    defaultSearch = (string)global["searchdefault"];
                    result = (string)global["current"];
                    if (string.IsNullOrEmpty(result))
                        return defaultSearch;
                    else
                        return result;
                }
                catch (Exception ex)
                {
                    return defaultSearch;
                }
            }
            return result;
        }
    }
}
