using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;
using IContentBase = Umbraco.Cms.Core.Models.IContentBase;

namespace DF22.Web.Routing.UrlSegmentProviders
{
    internal sealed class ProductUrlSegmentProvider : IUrlSegmentProvider
    {
        private readonly IShortStringHelper shortStringHelper;

        public ProductUrlSegmentProvider(IShortStringHelper shortStringHelper)
        {
            this.shortStringHelper = shortStringHelper;
        }

        public string? GetUrlSegment(IContentBase content, string? culture = null)
        {
            // Only apply this rule for product pages
            if (content.ContentType.Alias != Product.ModelTypeAlias)
            {
                return null;
            }

            var productSku = content.GetValue<string>("sku");

            if (string.IsNullOrWhiteSpace(productSku))
            {
                return null;
            }

            return $"product-{productSku!.ToUrlSegment(this.shortStringHelper)}";
        }
    }
}
