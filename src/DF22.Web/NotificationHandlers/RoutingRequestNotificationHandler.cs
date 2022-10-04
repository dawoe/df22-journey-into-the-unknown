using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.NotificationHandlers
{
    internal sealed class RoutingRequestNotificationHandler : INotificationHandler<RoutingRequestNotification>
    {
        public void Handle(RoutingRequestNotification notification)
        {
            if (notification.RequestBuilder.PublishedContent == null)
            {
                return;
            }

            // get the homepage
            var homePage = notification.RequestBuilder.PublishedContent.AncestorOrSelf<Home>();

            // check if we are under maintenance
            if (homePage?.IsUnderMaintenance != true)
            {
                return;
            }

            //if under maintenance get the maintenance page
            var maintenancePage = homePage.FirstChild<MaintenancePage>();

            if (maintenancePage == null)
            {
                return;
            }

            // we have a maintenance page, so we swap out the current content item with maintenance page.
            notification.RequestBuilder.SetPublishedContent(maintenancePage);
            notification.RequestBuilder.TrySetTemplate(maintenancePage.GetTemplateAlias());
        }
    }
}
