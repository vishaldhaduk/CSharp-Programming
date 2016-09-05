
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Lavasoft.SearchProtect.Business.Browsers.FireFox.Helpers
{
    public static class PreferenceHelper
    {
        private const string NewTab = "user_pref(\"browser.newtab.url\", ";
        private const string NewTabPage = "user_pref(\"browser.newtabpage.url\", ";
        private const string HomePage = "user_pref(\"browser.startup.homepage\", ";
        private const string DefaultEngine = "user_pref(\"browser.search.defaultenginename\", ";
        private const string DefaultEngineUS = "user_pref(\"browser.search.defaultenginename.US\", ";
        private const string SelectedEngine = "user_pref(\"browser.search.selectedEngine\", ";
        private const string BrowserStartUpPage = "user_pref(\"browser.startup.page\", ";
        private const string SearchSuggestion = "user_pref(\"browser.search.suggest.enabled\", ";
        private const string StartupPage = "user_pref(\"browser.startup.page\", ";
        private const string NewTabPageEnabled = "user_pref(\"browser.newtabpage.enabled\", ";
        private const string SessionRestoreOnce = "user_pref(\"browser.sessionstore.resume_session_once\", ";
        private const string ExtensionSignature = "user_pref(\"xpinstall.signatures.required\", ";

        public static string SetHomePage(this string prefs ,string name)
        {
            return prefs != null ? prefs.Set(HomePage, name) : String.Empty;
        }

        public static string SetSearchEngine(this string prefs, string name)
        {
            return prefs != null ? prefs.Set(DefaultEngine, name).Set(SelectedEngine, name) : String.Empty;
        }

        public static string SetNewTab(this string prefs, string name)
        {
            return prefs != null ? prefs
                .Set(NewTab, name) : String.Empty;
        }

        public static string SetNewTabPage(this string prefs, string name)
        {
            return prefs != null ? prefs
                .Set(NewTabPage, name) : String.Empty;
        }

        public static string SetSessionRestoreOnce(this string prefs, bool value) 
        {
            return prefs != null ? prefs.Set(SessionRestoreOnce, value) : String.Empty;
        }

        public static string SetExtensionFF43SetSignatureToFalse(this string prefs)
        {
            return prefs != null ? prefs.Set(ExtensionSignature, false) : String.Empty;
        }

        public static string SetSuggestion(this string prefs, bool value = true)
        {
            return prefs.Set(SearchSuggestion, value);
        }

        public static string GetHomePage(this string prefs)
        {
            return prefs.Get(HomePage);
        }

        public static string GetSearchEngine(this string prefs)
        {
            return prefs.Get(DefaultEngine);
        }

        public static string GetNewTab(this string prefs)
        {
            return prefs.Get(NewTab);
        }

        public static string BrowserStartUp(this string prefs, int value)
        {
            return prefs != null ? prefs.Set(BrowserStartUpPage, value) : String.Empty;
        }

        public static string DeleteStartup(this string prefs)
        {
            return prefs.Delete(StartupPage);
        }

        public static string DeleteStartupHomepage(this string prefs)
        {
            return prefs.Delete(HomePage);
        }

        public static string DeleteNewTab(this string prefs)
        {
            return prefs.Delete(NewTab);
        }

        public static string SetNewtabPageEnabled(this string prefs, bool value)
        {
            return prefs != null ? prefs.Set(NewTabPageEnabled, value) : String.Empty;
        }

        private static string Set(this string prefs, string key, string value)
        {
            try
            {
                int startIndexSetting = prefs.IndexOf(key, System.StringComparison.Ordinal);

                if (startIndexSetting == -1)
                {
                    List<string> settings = new List<string>();

                    using (var file = new StringReader(prefs))
                    {
                        string line = file.ReadLine();

                        while (line != null)
                        {
                            settings.Add(line);
                            line = file.ReadLine();
                        }
                    }

                    settings.Add(string.Format(key + "\"{0}\");", value));

                    string result = string.Empty;

                    foreach (var item in settings)
                    {
                        result += item + "\n";
                    }
                    return result;

                    //return prefs.Insert(0, string.Format(key + "\"{0}\");\n", value));
                }
                else
                {
                    int startIndexRemuve = startIndexSetting + key.Length + 1;
                    int endIndexRemuve = prefs.IndexOf("\"", startIndexRemuve, System.StringComparison.Ordinal);

                    return prefs
                        .Remove(startIndexRemuve, endIndexRemuve - startIndexRemuve)
                        .Insert(startIndexRemuve, value);
                }
            }
            catch (System.Exception ex)
            {                
                throw ex;
            }
        }
        private static string Set(this string prefs, string key, int value)
        {
            try
            {
                int startIndexSetting = prefs.IndexOf(key, System.StringComparison.Ordinal);

                if (startIndexSetting == -1)
                {
                    List<string> settings = new List<string>();

                    using (var file = new StringReader(prefs))
                    {
                        string line = file.ReadLine();

                        while (line != null)
                        {
                            settings.Add(line);
                            line = file.ReadLine();
                        }
                    }

                    string tobeInserted = string.Format(key + value.ToString().ToLower() + ");");

                    settings.Add(tobeInserted);

                    string result = string.Empty;

                    foreach (var item in settings)
                    {
                        result += item + "\n";
                    }
                    return result;
                    //return prefs.Insert(0, string.Format(key + "{0});\n", value));
                }
                else
                {
                    int startIndexRemuve = startIndexSetting + key.Length;
                    int endIndexRemuve = prefs.IndexOf(")", startIndexRemuve, System.StringComparison.Ordinal);

                    return prefs
                        .Remove(startIndexRemuve, endIndexRemuve - startIndexRemuve)
                        .Insert(startIndexRemuve, value.ToString());
                }
            }
            catch (System.Exception ex)
            {                
                throw ex;
            }
        }
        private static string Set(this string prefs, string key, bool value)
        {
            try
            {
                int startIndexSetting = prefs.IndexOf(key, System.StringComparison.Ordinal);

                if (startIndexSetting == -1)
                {
                    List<string> settings = new List<string>();

                    using (var file = new StringReader(prefs))
                    {
                        string line = file.ReadLine();

                        while (line != null)
                        {
                            settings.Add(line);
                            line = file.ReadLine();
                        }
                    }

                    string tobeInserted = string.Format(key + value.ToString().ToLower() + ");");

                    settings.Add(tobeInserted);

                    string result = string.Empty;

                    foreach (var item in settings)
                    {
                        result += item + "\n";
                    }
                    return result;
                    //return prefs.Insert(0, string.Format(key + "{0});\n", value));
                }
                else
                {
                    int startIndexRemuve = startIndexSetting + key.Length;
                    int endIndexRemuve = prefs.IndexOf(")", startIndexRemuve, System.StringComparison.Ordinal);

                    return prefs
                        .Remove(startIndexRemuve, endIndexRemuve - startIndexRemuve)
                        .Insert(startIndexRemuve, value.ToString().ToLower());
                }
            }
            catch (System.Exception ex) 
            {
                throw ex;
            }
        }

        private static string Get(this string prefs, string key)
        {
            try
            {
                int startIndexSetting = prefs.IndexOf(key, System.StringComparison.Ordinal);
                if (startIndexSetting == -1)
                {
                    key = DefaultEngineUS;
                    startIndexSetting = prefs.IndexOf(key, System.StringComparison.Ordinal);
                }
                if (startIndexSetting == -1)
                {
                    key = SelectedEngine;
                    startIndexSetting = prefs.IndexOf(key, System.StringComparison.Ordinal);
                }
                if (startIndexSetting == -1)
                {
                    return string.Empty;
                }
                else
                {
                    int startIndexRemuve = startIndexSetting + key.Length + 1;
                    int endIndexRemuve = prefs.IndexOf("\"", startIndexRemuve, System.StringComparison.Ordinal);
                    return prefs.Substring(startIndexRemuve, endIndexRemuve - startIndexRemuve);
                }
            }
            catch (System.Exception)
            {                
                throw;
            }
        }

        private static string Delete(this string prefs, string key)
        {
            int startIndexSetting = prefs.IndexOf(key, System.StringComparison.OrdinalIgnoreCase);
            if (startIndexSetting > 0)
            {
                int endIndexRemove = prefs.IndexOf(";", startIndexSetting + 1, System.StringComparison.OrdinalIgnoreCase);
                return prefs.Remove(startIndexSetting, endIndexRemove - startIndexSetting + 1);
            }

            return prefs; // the value is not there
        }
    }
}