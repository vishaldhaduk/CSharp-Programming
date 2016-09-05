using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lavasoft.SearchProtect.Business.Browsers.FireFox.Helpers
{
    /// <summary>
    /// plugin objects used for JSON convert.
    /// </summary>
    public class PluginJSON
    {

    }

    public class GlobalSearchMetadataJson
    {
        [JsonProperty("[global]")]
        public SearchMetadataJson metaData { get; set; }
    }

    public class SearchMetadataJson
    {
        [JsonProperty("current")]
        public string current { get; set; }

        [JsonProperty("hash")]
        public string hash { get; set; }

        [JsonProperty("searchdefault")]
        public string searchdefault { get; set; }

        [JsonProperty("searchdefaultexpir")]
        public long searchdefaultexpir { get; set; }

        [JsonProperty("searchdefaulthash")]
        public string searchdefaulthash { get; set; }

    }

    public class MozPlugin
    {
        [JsonProperty("_name")]
        public string name { get; set; }

        [JsonProperty("_shortName")]
        public string shortName { get; set; }

        [JsonProperty("_loadPath")]
        public string loadPath { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("__searchForm")]
        public string __searchForm { get; set; }

        [JsonProperty("_iconURL")]
        public string _iconURL { get; set; }

        [JsonProperty("_iconMapObj")]
        public MozImgs _iconMapObj { get; set; }

        [JsonProperty("_metadata")]
        public EmptySpace metadata { get; set; }

        [JsonProperty("_urls")]
        public MozUrls[] _urls { get; set; }

        [JsonProperty("queryCharset")]
        public string queryCharset { get; set; }

        [JsonProperty("_readOnly")]
        public bool readOnly { get; set; }

        [JsonProperty("filePath")]
        public string filePath { get; set; }
    }

    public class MozImgs
    {
        [JsonProperty("{\"width\":16,\"height\":16}")]
        public string w16 { get; set; }

        [JsonProperty("{\"width\":65,\"height\":26}")]
        public string w65 { get; set; }

        [JsonProperty("{\"width\":130,\"height\":52}")]
        public string w130 { get; set; }
    }

    public class MozUrls
    {
        [JsonProperty("template")]
        public string template { get; set; }

        [JsonProperty("rels")]
        public string[] rels { get; set; }

        [JsonProperty("resultDomain")]
        public string resultDomain { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("params")]
        public MozParams[] @params { get; set; }
    }

    public class MozParams
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }
    }

    public class EmptySpace
    {

    }

    public class SearchPlugins
    {
        [JsonProperty("SearchPlugin")]
        public PluginJSONRAW searchPlugin { get; set; }
    }

    public class PluginJSONRAW
    {
        [JsonProperty("ShortName")]
        public string shortName { get; set; }

        [JsonProperty("Description")]
        public string description { get; set; }

        [JsonProperty("SearchForm")]
        public string __searchForm { get; set; }
        
        [JsonProperty("Image")]
        [JsonConverter(typeof(SingleOrArrayConverter<ImagePluginRaw>))]
        public ImagePluginRaw[] Image { get; set; }

        [JsonProperty("Url")]
        [JsonConverter(typeof(SingleOrArrayConverter<SearchUrlsRaw>))]
        public SearchUrlsRaw[] _urls { get; set; }

        [JsonProperty("InputEncoding")]
        public string queryCharset { get; set; }

        
    }

    public class SearchUrlsRaw
    {
        [JsonProperty("@template")]
        public string template { get; set; }
        [JsonProperty("@type")]
        public string type { get; set; }
        [JsonProperty("Param")]
        [JsonConverter(typeof(SingleOrArrayConverter<UrlParamsRaw>))]
        public UrlParamsRaw[] @params { get; set; }
    }

    public class UrlParamsRaw
    {
        [JsonProperty("@name")]
        public string name { get; set; }
        [JsonProperty("@value")]
        public string value { get; set; }
    }

    public class ImagePluginRaw
    {
        [JsonProperty("@width")]
        public string width { get; set; }

        [JsonProperty("@height")]
        public string height { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(T[]));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<T[]>();
            }
            return new T[] { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
