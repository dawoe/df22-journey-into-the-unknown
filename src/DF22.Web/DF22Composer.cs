using DF22.Web.BackOffice;
using DF22.Web.NotificationHandlers;
using DF22.Web.Routing.ContentFinders;
using DF22.Web.Routing.UrlProviders;
using DF22.Web.Routing.UrlSegmentProviders;
using DF22.Web.Routing.VirtualRoutes;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Cms.Web.Common.Routing;

namespace DF22.Web
{
    internal sealed class DF22Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // register notification handlers
            builder
                .AddNotificationHandler<SendingAllowedChildrenNotification,
                    SendingAllowedChildrenNotificationHandler>();
            builder.AddNotificationHandler<MenuRenderingNotification, MenuRenderingNotificationHandler>();
            builder.AddNotificationHandler<SendingContentNotification, SendingContentNotificationHandler>();
            builder.AddNotificationHandler<RoutingRequestNotification, RoutingRequestNotificationHandler>();

            // register custom search fields
            builder.Services.AddUnique<IUmbracoTreeSearcherFields, CustomSearchFields>();

            // register routing extensions
            builder.UrlSegmentProviders().Insert<ProductUrlSegmentProvider>();
            builder.UrlProviders().Insert<BlogUrlProvider>();
            builder.ContentFinders().Append<BlogContentFinder>();

            //handle sitemap.xml as server side request
            builder.Services.Configure<UmbracoRequestOptions>(options =>
            {
                var allowList = new[] { "/sitemap.xml" };
                options.HandleAsServerSideRequest = httpRequest =>
                {
                    foreach (var route in allowList)
                    {
                        if (httpRequest.Path.StartsWithSegments(route))
                        {
                            return true;
                        }
                    }

                    return false;
                };
            });

            // register virtual route for server side request
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(SitemapController))
                {
                    Endpoints = app => app.UseEndpoints(endpoints => endpoints.MapControllerRoute(
                        "Xml SiteMap Controller",
                        "/sitemap.xml",
                        new { Controller = "Sitemap", Action = "Index" })),
                });
            });
        }
    }
}
