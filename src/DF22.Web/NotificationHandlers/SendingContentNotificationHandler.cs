using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.NotificationHandlers
{
    internal sealed class SendingContentNotificationHandler : INotificationHandler<SendingContentNotification>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        public SendingContentNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        public void Handle(SendingContentNotification notification)
        {
            this.PreventUnPublishing(notification);

            var user = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;

            if (user != null && user.Groups.Any(x => x.Alias == Umbraco.Cms.Core.Constants.Security.AdminGroupAlias))
            {
                // don't hide for admins
                return;
            }

            this.HideMaintenanceProperty(notification);
        }

        private void HideMaintenanceProperty(SendingContentNotification notification)
        {
            if (notification.Content.ContentTypeAlias.Equals(Home.ModelTypeAlias))
            {
                foreach (var variant in notification.Content.Variants)
                {
                    foreach (var tab in variant.Tabs)
                    {
                        tab.Properties =
                            (tab.Properties ?? Array.Empty<ContentPropertyDisplay>()).Where(x =>
                                x.Alias != "isUnderMaintenance");
                    }
                }
            }
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
