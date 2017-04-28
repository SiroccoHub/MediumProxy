using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading.Tasks;
using MediumProxy.Model;
using Microsoft.Extensions.Logging;

namespace MediumProxy
{
    public class MediumFeedParser
    {
        private readonly ILogger _logger = ApplicationLogging.CreateLogger<MediumFeedParser>();

        public async Task<Channel> Parse(string xmlString)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ReplaceHexadecimalSymbols(xmlString));
                var doc = xmlDoc.DocumentElement.SelectSingleNode("/rss/channel");

                var channel = new Channel
                {
                    Title = doc.SelectSingleNode("title").InnerText,
                    Description = doc.SelectSingleNode("description").InnerText,
                    Link = new Uri(doc.SelectSingleNode("link").InnerText),
                    Image = new Image
                    {
                        Uri = new Uri(doc.SelectSingleNode("image/url").InnerText),
                        Title = doc.SelectSingleNode("image/title").InnerText,
                        Link = new Uri(doc.SelectSingleNode("image/link").InnerText),
                    },
                    Generator = doc.SelectSingleNode("generator").InnerText,
                    LastBuildDate = DateTime.Parse(doc.SelectSingleNode("lastBuildDate").InnerText),
                    WebMaster = doc.SelectSingleNode("webMaster").InnerText
                };

                var items = (from XmlNode node in doc.SelectNodes("item")
                    select new Item(node.SelectSingleNode("guid").InnerText)
                    {
                        Title = node.SelectSingleNode("title").InnerText,
                        Link = new Uri(node.SelectSingleNode("link").InnerText),
                        ContentEncoded = node.SelectSingleNode("*[name()='content:encoded']").InnerText,
                        Categories = null,
                        DcCreator = node.SelectSingleNode("*[local-name()='creator']").InnerText,
                        PubDate = DateTime.Parse(node.SelectSingleNode("pubDate").InnerText),
                        AtomUpdated = DateTime.Parse(node.SelectSingleNode("*[local-name()='updated']").InnerText)
                    }).ToList();

                channel.Items = items;
                return channel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return new Channel();
            }
        }

        private static string ReplaceHexadecimalSymbols(string txt)
        {
            const string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }
    }
}
