using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Xml.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.Routing.VirtualRoutes
{
    public class SitemapController : UmbracoPageController, IVirtualPageController
    {
        private readonly XNamespace xmlNameSpace = "http://www.sitemaps.org/schemas/sitemap/0.9";


        public SitemapController(ILogger<UmbracoPageController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine)
        {
        }

        public IActionResult Index()
        {
            return this.Content(this.GetSiteMapContents(), "text/xml");
        }

        public IPublishedContent? FindContent(ActionExecutingContext actionExecutingContext)
        {
            var umbracoContextAccessor = actionExecutingContext.HttpContext.RequestServices
                .GetRequiredService<IUmbracoContextAccessor>();

            if (umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == false || umbracoContext?.Content == null)
            {
                return null;
            }

            return umbracoContext.Content.GetAtRoot().OfType<Home>().FirstOrDefault();
        }

        private List<(string Url, DateTime LastModified)> GetSiteMapItems()
        {
            var pagesForSiteMap =
                this.CurrentPage?.DescendantsOrSelf().Where(x => x.IsDocumentType(MaintenancePage.ModelTypeAlias) == false)
                    .ToList() ?? new List<IPublishedContent>();

            var siteMapItems = new List<(string Url, DateTime LastModified)>();

            foreach (var page in pagesForSiteMap)
            {
                siteMapItems.Add(new ValueTuple<string, DateTime>(page.Url(mode: UrlMode.Absolute), page.UpdateDate));
            }

            return siteMapItems;
        }

        private string GetSiteMapContents()
        {
            var siteMapItems = GetSiteMapItems();

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(
                    this.xmlNameSpace + "urlset",
                    this.GetSiteMapItems().Select(this.CreateItemElement)));

            return sitemap.ToString();
        }

        private XElement CreateItemElement((string Url, DateTime LastModified) item)
        {
            var itemElement = new XElement(this.xmlNameSpace + "url");

            itemElement.Add(new XElement(this.xmlNameSpace + "loc", item.Url));
            itemElement.Add(new XElement(this.xmlNameSpace + "lastmod", item.LastModified.ToString("yyyy-MM-ddTHH:mm:ss.f") + "+00:00"));

            return itemElement;
        }
    }
}
