using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediumProxy.Model;

namespace MediumProxy
{
    public static class MediumProxyExtentions
    {
        private static readonly Regex RegexFirstImageUrl = new Regex("<img[^<]+src=\"([^<\"]+)\"[^<]+>");

        public static async Task<Uri> GetFirstImageUriAsync(this Item item, Uri defaultUrl = null)
        {
            if (string.IsNullOrEmpty(item?.Description)) return defaultUrl;

            var match = RegexFirstImageUrl.Match(item.Description);

            if (match.Groups.Count < 1) return defaultUrl;

            try
            {
                return new Uri(match.Groups[1].Value);
            }
            catch (Exception ex)
            {
                return defaultUrl;
            }
        }
    }
}
