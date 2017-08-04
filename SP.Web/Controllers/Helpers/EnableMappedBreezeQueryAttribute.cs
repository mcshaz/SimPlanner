
using Breeze.WebApi2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData.Query;

namespace SP.Web.Controllers.Helpers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class EnableMappedBreezeQueryAttribute : EnableBreezeQueryAttribute
    {
        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            var queryHelper = GetQueryHelper(queryOptions.Request);
            if (queryOptions.SelectExpand?.RawExpand != null)
            {
                queryOptions = QueryHelper.RemoveOptions(queryOptions, new List<string>() { "$expand" });
                //queryOptions = new ODataQueryOptions()
            }
            return queryHelper.ApplyQuery(queryable, queryOptions);
        }
    }
}