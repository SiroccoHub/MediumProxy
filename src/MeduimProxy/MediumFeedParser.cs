using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using MeduimProxy.Model;
using Microsoft.Extensions.Logging;

namespace MeduimProxy
{
    public class MediumFeedParser
    {
        private readonly ILogger _logger = ApplicationLogging.CreateLogger<MediumFeedParser>();

        public async Task<Channel> Parse(string xmlString)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);
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

                var items = new List<Item>();

                foreach (XmlNode node in doc.SelectNodes("item"))
                {
                    items.Add(new Item(node.SelectSingleNode("guid").InnerText)
                    {
                        Title = node.SelectSingleNode("title").InnerText,
                        Description = node.SelectSingleNode("description").InnerText,
                        Link = new Uri(node.SelectSingleNode("link").InnerText),
                        Categories = null,
                        DcCreator = node.SelectSingleNode("*[local-name()='creator']").InnerText,
                        PubDate = DateTime.Parse(node.SelectSingleNode("pubDate").InnerText),
                        AtomUpdated = DateTime.Parse(node.SelectSingleNode("*[local-name()='updated']").InnerText)
                    });
                }

                channel.Items = items;
                return channel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return new Channel();
            }
        }
    }
}
