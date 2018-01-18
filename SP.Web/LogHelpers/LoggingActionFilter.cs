using NLog;
using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;


namespace SP.Web.LogHelpers
{
    /// <summary>
    /// ASP.NET Web API exception filter for logging errors to ELMAH. This
    /// assumes an HttpContext is present (i.e., running under ASP.NET - it
    /// will not work as a self-hosted WCF application).
    /// </summary>
    /// <remarks>
    /// Original implementation from http://stackoverflow.com/questions/766610/how-to-get-elmah-to-work-with-asp-net-mvc-handleerror-attribute/779961#779961.
    /// Ported from the Elmah.Contrib.Mvc package on NuGet.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class LoggingActionFilter : ActionFilterAttribute
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            _logger.Debug(() => "ACTION ARGUMENTS:\r\n" + string.Join("\r\n", actionExecutedContext.ActionContext.ActionArguments.Select(aa => $"key:{aa.Key}\r\n\tvalue:{aa.Value}")));
            var e = actionExecutedContext.Exception;
            if (e != null /*&& !e.IsLogged()*/)
            {
                _logger.Error(e, "HttpActionExecutedContext.Exception");
                e.MarkAsLogged();
            }
            else if ((int)actionExecutedContext.Response.StatusCode >= 500)
            {
                e = new HttpException(
                        (int)actionExecutedContext.Response.StatusCode,
                        ResolveMessage(actionExecutedContext));
                _logger.Error(e, "Internal Server Error Returned");
                e.MarkAsLogged();
            }
        }

        private string ResolveMessage(HttpActionExecutedContext actionExecutedContext)
        {
            const string messageKey = "Message";

            var defaultMessage = actionExecutedContext.Response.ReasonPhrase;
            var objectContent = actionExecutedContext.Response.Content as ObjectContent<HttpError>;
            if (objectContent == null) return defaultMessage;

            var value = objectContent.Value as HttpError;
            if (value == null) return defaultMessage;

            if (!value.ContainsKey(messageKey)) return defaultMessage;

            var message = value[messageKey] as string;
            return string.IsNullOrWhiteSpace(message) ? defaultMessage : message;
        }
    }
}

