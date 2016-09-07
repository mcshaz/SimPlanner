using Breeze.WebApi2;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http.OData.Query;

namespace SP.Web.Controllers.Helpers
{
    public class EnableMappedBreezeQueryAttribute : EnableBreezeQueryAttribute
    {
        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            if (queryOptions.SelectExpand?.RawExpand != null)
            {
                var url = queryOptions.Request.RequestUri.AbsoluteUri;

                url = Regex.Replace(url, @"\$expand=[^&]+&", "");
                var req = new HttpRequestMessage(HttpMethod.Get, url);

                queryOptions = new ODataQueryOptions(queryOptions.Context, req); 
                //queryOptions = new ODataQueryOptions()
            }

            return base.ApplyQuery(queryable, queryOptions);
        }
    }
}