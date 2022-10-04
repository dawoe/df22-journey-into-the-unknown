using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.Search;

namespace DF22.Web.BackOffice
{
    internal sealed class CustomSearchFields : UmbracoTreeSearcherFields, IUmbracoTreeSearcherFields
    {
        public CustomSearchFields(ILocalizationService localizationService) : base(localizationService)
        {
        }

        //Adding custom field to search in document types
        public new IEnumerable<string> GetBackOfficeDocumentFields()
        {
            return new List<string>(base.GetBackOfficeDocumentFields()) { "sku", "excerpt" };
        }
    }
}
