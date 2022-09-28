using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.NotificationHandlers
{
    internal sealed class SendingContentNotificationHandler : INotificationHandler<SendingContentNotification>
    {
        public void Handle(SendingContentNotification notification)
        {
            this.PreventUnPublishing(notification);
        }

        private void PreventUnPublishing(SendingContentNotification notification)
        {
            if (notification.Content.ContentTypeAlias.Equals(Home.ModelTypeAlias) ||
                notification.Content.ContentTypeAlias.Equals(MaintenancePage.ModelTypeAlias))
            {
                // for the homepage and maintenance page we will remove the un publish action
                // they are too important, and un-publishing will break our website
                var allowedActions = (notification.Content.AllowedActions ?? Array.Empty<string>()).ToList();

                allowedActions.Remove(Umbraco.Cms.Core.Actions.ActionUnpublish.ActionLetter.ToString());

                notification.Content.AllowedActions = allowedActions;
            }
        }
    }
}
