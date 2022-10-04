﻿using DF22.Web.BackOffice;
using DF22.Web.NotificationHandlers;
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

            // register custom search fields
            builder.Services.AddUnique<IUmbracoTreeSearcherFields, CustomSearchFields>();
        }
    }
}
