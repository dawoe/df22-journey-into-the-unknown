using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.NotificationHandlers
{
    internal sealed class MenuRenderingNotificationHandler : INotificationHandler<MenuRenderingNotification>
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public MenuRenderingNotificationHandler(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public void Handle(MenuRenderingNotification notification)
        {
            // find the docs at
            // https://our.umbraco.com/documentation/Extending/Section-Trees/Trees/#menurenderingnotification
            
            if (notification.TreeAlias.Equals(Umbraco.Cms.Core.Constants.Trees.Content) == false)
            {
                // not on the content tree
                return;
            }

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == false 
                || umbracoContext.Content == null)
            {
                return;
            }

            // we use the content cache in preview mode here, because that is faster than using content service.
            var content = umbracoContext.Content.GetById(true, int.Parse(notification.NodeId));

            if (content == null 
                || (content.IsDocumentType(Home.ModelTypeAlias) == false 
                && content.IsDocumentType(MaintenancePage.ModelTypeAlias) == false))
            {
                return;
            }

            // we are on a home or maintenance page
            // so we will remove the delete action 
            // see https://our.umbraco.com/documentation/Extending/Section-Trees/Trees/tree-actions
            notification.Menu.Items.RemoveAll(x => x.Alias == Umbraco.Cms.Core.Actions.ActionDelete.ActionAlias);
        }
    }
}
