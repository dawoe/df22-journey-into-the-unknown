using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DF22.Web.Routing.ContentFinders
{
    internal sealed class BlogContentFinder : IContentFinder
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public BlogContentFinder(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {
            if (this._umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == false || umbracoContext?.Content == null)
            {
                return Task.FromResult(false);
            }

            // get the blog overview page
            var blogOverviewPage = umbracoContext.Content.GetAtRoot().FirstOrDefault()?.FirstChild<Blog>();

            if (blogOverviewPage == null)
            {
                return Task.FromResult(false);
            }

            var path = request.Uri.GetAbsolutePathDecoded();

            if (path.StartsWith(blogOverviewPage.Url(mode: UrlMode.Relative)) == false)
            {
                // if the path does not start with the blog post overview page url, this is not a blog post
                return Task.FromResult(false);
            }
            
            // get the last segment of the url
            var urlSegment = path.Split("/", StringSplitOptions.RemoveEmptyEntries).Last();

            // get the blog post with the same url segment
            var blogPost = blogOverviewPage.Children?.Where(x => x.UrlSegment != null && x.UrlSegment.Equals(urlSegment))
                .FirstOrDefault();

            if (blogPost == null)
            {
                return Task.FromResult(false);
            }

            request.SetPublishedContent(blogPost);

            return Task.FromResult(true);
        }
    }
}
