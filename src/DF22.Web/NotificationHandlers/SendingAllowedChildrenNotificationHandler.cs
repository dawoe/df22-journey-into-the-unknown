using System.Web;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.NotificationHandlers
{
    internal sealed class SendingAllowedChildrenNotificationHandler : INotificationHandler<SendingAllowedChildrenNotification>
    {
        public void Handle(SendingAllowedChildrenNotification notification)
        {
            var queryStringCollection = HttpUtility.ParseQueryString(notification.UmbracoContext.OriginalRequestUrl.Query);

            // try get the id from the content item in the back office 
            // from which the create dialog is triggered.
            if (!queryStringCollection.ContainsKey("contentId"))
            {
                return;
            }

            var contentId = queryStringCollection["contentId"].TryConvertTo<int>().ResultOr(-1);

            if (contentId == -1)
            {
                return;
            }

            var content = notification.UmbracoContext.Content?.GetById(true, contentId);

            if (content == null)
            {
                return;
            }

            if (content.ContentType.Alias.Equals(Home.ModelTypeAlias) == false)
            {
                // not the homepage, so we don't need to check
                return;
            }

            // gets the current list of document types allowed as children of homepage.
            var allowedChildren = notification.Children.ToList();

            if (content.ChildrenForAllCultures != null)
            {
                // get all children of the homepage.
                var childNodes = content.ChildrenForAllCultures.ToList();

                // if there is a maintenance page already created, then don't allow it anymore
                if (childNodes.Any(x => x.ContentType.Alias == MaintenancePage.ModelTypeAlias))
                {
                    allowedChildren.RemoveAll(x => x.Alias == MaintenancePage.ModelTypeAlias);
                }
            }

            // update the allowed children.
            notification.Children = allowedChildren;
        }
    }
}
