using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediumProxy.Model
{
    public class Item : IEquatable<Item>
    {
        public Item(string mediumGuid)
        {
            MediumGuid = mediumGuid;
            Categories = new List<string>();
        }

        public string Title { get; set; }
        public string ContentEncoded { get; set; }
        public Uri Link { get; set; }

        public string MediumGuid { get; }

        public List<string> Categories { get; set; }

        public string DcCreator { get; set; }

        /// <summary>
        /// feed e.g. feed e.g. Fri, 04 Nov 2016 23:40:43 GMT 
        /// </summary>
        public DateTime PubDate { get; set; }

        /// <summary>
        /// feed e.g. 2016-11-04T23:40:43.364Z
        /// </summary>
        public DateTime AtomUpdated { get; set; }

        public override string ToString()
        {
            return MediumGuid;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var objItem = obj as Item;
            return objItem != null && Equals(objItem);
        }
        public override int GetHashCode()
        {
            return MediumGuid.GetHashCode();
        }
        public bool Equals(Item other)
        {
            return other != null && (MediumGuid.Equals(other.MediumGuid));
        }

    }
}
