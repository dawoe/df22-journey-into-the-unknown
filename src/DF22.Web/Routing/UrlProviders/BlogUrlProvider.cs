using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.Routing.UrlProviders
{
    internal sealed class BlogUrlProvider : IUrlProvider
    {
        public UrlInfo? GetUrl(IPublishedContent content, UrlMode mode, string? culture, Uri current)
        {
            if (content is not Blogpost blogpost)
            {
                return null;
            }

            var path = $"{blogpost.Parent!.Url(mode : mode).EnsureEndsWith("/")}{blogpost.CreateDate.Year}/{blogpost.UrlSegment}/";

            return new UrlInfo(path, true, culture);
        }

        public IEnumerable<UrlInfo> GetOtherUrls(int id, Uri current)
        {
            return new List<UrlInfo>();
        }
    }
}
