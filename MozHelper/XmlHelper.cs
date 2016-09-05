using System;
using System.Linq;
using System.Xml.Linq;

namespace Lavasoft.SearchProtect.Business.Browsers.FireFox.Helpers
{
    public static class XmlHelper
    {
        public static string GetShortName(this string fileName)
        {
            try
            {
                var doc = XDocument.Load(fileName);
                var elements = from el in doc.Descendants() select el;

                foreach (var xNode in elements.Nodes())
                {
                    var element = (XElement) xNode;
                    if (element.Name.LocalName == "ShortName")
                    {
                        return element.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //log.Error("Error in getting search name" + ex.Message);
                throw new ArgumentException(string.Format("File {0} Not Find", fileName));
            }
            return string.Empty;
        }
    }
}
