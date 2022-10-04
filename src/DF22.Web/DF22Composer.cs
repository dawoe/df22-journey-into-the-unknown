using DF22.Web.BackOffice;
using DF22.Web.NotificationHandlers;
using DF22.Web.Routing.UrlProviders;
using DF22.Web.Routing.UrlSegmentProviders;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;

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

            // register routing stuff
            builder.UrlSegmentProviders().Insert<ProductUrlSegmentProvider>();
            builder.UrlProviders().Insert<BlogUrlProvider>();
        }
    }
}
