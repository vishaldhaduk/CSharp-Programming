using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml;
using LZ4;
using System.Diagnostics;
using Lavasoft.SearchProtect.Business.Browsers.FireFox.Helpers;

namespace MozHelper
{
    public class Mozlz4Helper
    {
        private static string[] visibleDefaultEngines = {"amazondotcom",
        "bing",
        "eBay",
        "google",
        "twitter",
        "wikipedia",
        "yahoo",
        "ddg"};

        private static string _BackupPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Lavasoft\\Web Companion\\Options\\b_search.json";

        /// <summary>
        /// new logic to set search for FF46 and up.
        /// </summary>
        /// <param name="pluginPath"> search plugin choosen by user </param>
        public static void SetSearchFF46(string pluginPath)
        {
            try
            {
                string json = CreateJsonObjects(FirefoxPath.SearchMetadataFilePath, pluginPath, _BackupPath);
                //CreateSearchJsonFile(json, FirefoxPath.SearchJsonFilePath);
                CreateMozLZ4File(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failure to create file and set search for Firefox 46 and plus");
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Created backup json file for reuse and so we don't have to decompress everytime
        /// or simply reuse backup json we used earlier
        /// </summary>
        /// <param name="backupJsonPath"></param>
        /// <returns>string of the backup search json path</returns>
        private static string FindBackupSearchJson(string backupJsonPath)
        {
            try
            {
                if (!File.Exists(backupJsonPath))
                {
                    string newJson = ReadMozFile(@"C:\Users\vishal.dhaduk.LAVASOFT\AppData\Roaming\Mozilla\Firefox\Profiles\9xca3ebe.default\search.json.mozlz4");
                    if (!string.IsNullOrEmpty(newJson))
                    {

                        CreateSearchJsonFile(newJson, backupJsonPath);

                        if (!File.Exists(backupJsonPath))
                        {
                            throw new Exception("Unable to create file");
                        }
                        else
                        {
                            return backupJsonPath;
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to create file");
                    }
                }
                else
                {
                    return backupJsonPath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot open or create backup Json");
                Debug.WriteLine(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// compress and create a search.json.mozlz4 file
        /// </summary>
        /// <param name="finalJson"> final search json string created for our search</param>
        private static void CreateMozLZ4File(string finalJson)
        {
            try
            {
                string mozPath = FirefoxPath.SearchJsonlz4FilePath;
                //FirefoxPath.SearchJsonlz4FilePath
                byte[] buffer = ConvertToByteArray(finalJson, Encoding.Default);
                byte[] compress = LZ4Codec.Encode(buffer, 0, buffer.Length);

                if (File.Exists(mozPath))
                {
                    File.Delete(mozPath);
                }
                File.Create(mozPath).Close();

                using (StreamWriter sw = new StreamWriter(mozPath))
                {
                    string mozlz4 = "mozlz4\0" + Encoding.Default.GetString(compress);
                    sw.Write(Encoding.Default.GetString(compress));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot create Mozlz4 file");
                throw ex;
            }
        }

        /// <summary>
        /// decompress mozlz4 file and turn search json string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReadMozFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.OpenRead(path))
                    {

                        int i = (int)fs.Length;
                        byte[] MozBuffer = new byte[8];
                        fs.Read(MozBuffer, 0, 8);
                        byte[] moz = ConvertToByteArray("mozLz40\0", Encoding.Default);
                        bool x = moz.SequenceEqual(MozBuffer);

                        fs.Read(new byte[4], 0, 4);

                        byte[] buffer = new byte[fs.Length - 12];
                        int bufferLength = buffer.Length;
                        fs.Read(buffer, 0, buffer.Length);


                        byte[] bufferOut = DecompressBytes(buffer);

                        return Encoding.Default.GetString(bufferOut);
                    }
                }
                else
                {
                    throw new Exception("Mozlz4 firefox file does not exists.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Decode Mozlz4 file.");
                Debug.WriteLine(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// LZ4 decompression wrapper
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        private static byte[] DecompressBytes(byte[] encoded)
        {
            if (encoded == null || encoded.Length == 0)
            {
                throw new ArgumentException("encoded data is null or size is 0");
            }

            byte[] decoded;

            // estimate, 10 times the compressed size
            byte[] buffer = new byte[encoded.Length * 10];

            try
            {
                int decodedsize = LZ4Codec.Decode(encoded, 0, encoded.Length, buffer, 0, buffer.Length, false);
                //buffer = Lz4Net.Lz4.DecompressBytes(encoded);
                if (decodedsize > 0)
                {
                    decoded = new byte[decodedsize];
                    Array.Copy(buffer, decoded, decodedsize);
                }
                else
                {
                    throw new InvalidOperationException("Failed to decode");
                }
                return decoded;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return new byte[0];
            }
        }

        /// <summary>
        /// converts string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// convert XML converted to json string to proper plugin json
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="pluginPath"></param>
        /// <returns></returns>
        private static MozPlugin RawPluginToMozPlugin(PluginJSONRAW raw, string pluginPath)
        {
            try
            {
                MozPlugin moz = new MozPlugin
                {
                    name = raw.shortName,
                    shortName = Path.GetFileNameWithoutExtension(pluginPath),
                    loadPath = "[profile]/searchplugins/" + Path.GetFileName(pluginPath),
                    description = raw.description,
                    __searchForm = raw.__searchForm,
                    _iconURL = raw.Image[0].text,
                    _iconMapObj = FillIconMap(raw.Image),
                    metadata = new EmptySpace(),
                    _urls = FillUrls(raw._urls),
                    queryCharset = raw.queryCharset,
                    readOnly = false,
                    filePath = pluginPath
                };

                return moz;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("problem creating mozPlugin Object");
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// add icons in another format to final json object
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private static MozImgs FillIconMap(ImagePluginRaw[] raw)
        {
            try
            {
                MozImgs moz = new MozImgs();
                if (raw.Length == 3)
                {
                    moz.w16 = raw[0].text;
                    moz.w65 = raw[1].text;
                    moz.w130 = raw[2].text;
                }
                else if (raw.Length == 2)
                {
                    moz.w16 = raw[0].text;
                    moz.w65 = raw[1].text;
                }
                else
                {
                    moz.w16 = raw[0].text;
                }
                return moz;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// add urls for search in final json
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private static MozUrls[] FillUrls(SearchUrlsRaw[] raw)
        {
            try
            {
                MozUrls[] moz = new MozUrls[raw.Length];
                Uri uri;
                for (int i = 0; i < raw.Length; i++)
                {
                    uri = new Uri(raw[i].template);
                    MozUrls mozu = new MozUrls
                    {
                        template = raw[i].template,
                        rels = InitiateRels(raw[i].type),
                        resultDomain = uri.Host,
                        @params = FillParams(raw[i].@params),
                        type = raw[i].type
                    };
                    moz[i] = mozu;
                }
                return moz;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot open or create backup Json");
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// adda relation to html type url and none to json seach suggestions
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string[] InitiateRels(string type)
        {
            if (type.IndexOf("html", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string[] stringArray = { "searchform" };
                return stringArray;
            }
            else
            {
                string[] emptyArray = { };
                return emptyArray;
            }
        }

        /// <summary>
        /// add array of objects parameters for search urls
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private static MozParams[] FillParams(UrlParamsRaw[] raw)
        {
            try
            {
                MozParams[] moz = new MozParams[raw.Length];
                for (int i = 0; i < raw.Length; i++)
                {
                    MozParams mozu = new MozParams
                    {
                        name = raw[i].name,
                        value = raw[i].value
                    };
                    moz[i] = mozu;
                }
                return moz;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot create MozParams objects");
                Debug.WriteLine(ex);
                return null;
            }
        }
        /*public static JObject DeletePlugin(JObject plugins)
        {
            bool hit = true;
            JArray jEngines = (JArray)plugins["engines"];
            List<string> pluginList = new List<string>();
            foreach(var job in jEngines)
            {
                JToken value = (JToken)job["_shortName"];
                for(int i = 0; i < visibleDefaultEngines.Length ; i++)
                {
                    if (visibleDefaultEngines[i].Contains(value.Value<string>))
                    {

                    }
                }
            }
        }*/

        /// <summary>
        /// 1. convert xml plugin to json
        /// 2. open and read string of metadata json
        /// 3. read backup search json to string
        /// 4. replace metadata json in search with our metadata
        /// 5. add xml converted plugin to search json plugin engine list.
        /// 6. return final json string to be placed in mozlz4 file
        /// </summary>
        /// <param name="metaJsonPath"></param>
        /// <param name="pluginPath"></param>
        /// <param name="backupSearch"></param>
        /// <returns></returns>
        private static string CreateJsonObjects(string metaJsonPath, string pluginPath, string backupSearch)
        {
            //string s = XmlHelper.OutputToZipPluginJSONObjects(@"C:\project\DEV\WebCompanion\SearchProtect.Business\Resources\bing-lavasoft.xml");
            //get assets and convert them in jsons
            XmlDocument doc = new XmlDocument();
            string xml = File.ReadAllText(pluginPath);
            string searchJson = File.ReadAllText(FindBackupSearchJson(backupSearch));
            doc.LoadXml(xml);
            string jsonPlugin = JsonConvert.SerializeXmlNode(doc);
            string metaJson = File.ReadAllText(metaJsonPath);

            try
            {
                //create plugin jsonobject
                SearchPlugins raw = JsonConvert.DeserializeObject<SearchPlugins>(jsonPlugin);
                //create meta data json object
                GlobalSearchMetadataJson meta = JsonConvert.DeserializeObject<GlobalSearchMetadataJson>(metaJson);
                //only need to parse searchjson
                JObject code = JObject.Parse(searchJson);
                //check for extra search plugin and delete
                //no need. for now, we backup a normal clean search.json, we will add a backup of the last successful plugin in the future.

                //replace metadata part with our own metadata
                JObject value = (JObject)code["metaData"];
                value = JObject.FromObject(meta.metaData);
                code["metaData"] = value;

                //add engine plugin in search mozlz4 json format
                JArray jar = (JArray)code["engines"];
                jar.Add(JObject.FromObject(RawPluginToMozPlugin(raw.searchPlugin, pluginPath)));
                code["engines"] = jar;

                string jsonFinal = JsonConvert.SerializeObject(code);

                return jsonFinal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot open or create backup Json");
                throw ex;
            }
        }

        /// <summary>
        /// creates a json file with a json string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="outputPath"></param>
        public static void CreateSearchJsonFile(string jsonString, string outputPath)
        {
            try
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }

                using (FileStream fs = File.Open(outputPath, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(jsonString);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot create json file");
                Debug.WriteLine(ex);
            }
        }

        public static string GetCurrentSearch()
        {
            try
            {
                //open mozlz4
                string lz4 = ReadMozFile(FirefoxPath.SearchJsonlz4FilePath);

                JObject code = JObject.Parse(lz4);
                JObject meta = (JObject)code["metaData"];
                string search = (string)meta["current"];
                if (!string.IsNullOrEmpty(search))
                    return search;
                else
                    return "google";
                //get engine
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot get search");
                Debug.WriteLine(ex);
                return "google";
            }
        }
    }
}
