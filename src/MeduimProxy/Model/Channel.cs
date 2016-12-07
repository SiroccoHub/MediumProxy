using System;
using System.Collections.Generic;

namespace MeduimProxy.Model
{
    public class Channel
    {
        public Channel()
        {
            Items = new List<Item>();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Link { get; set; }
        public Image Image { get; set; }
        public string Generator { get; set; }
        public DateTime LastBuildDate { get; set; }
        public string WebMaster { get; set; }
        public List<Item> Items { get; set; }
    }
}