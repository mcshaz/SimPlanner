using NLog;
using System;
using System.Web.Http.ExceptionHandling;


namespace SP.Web.LogHelpers
{

    public class NLogExceptionLogger : ExceptionLogger
    {

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public override void Log(ExceptionLoggerContext context)
        {
            if (!context.Exception.IsLogged())
                _log.Error(context.Exception);
        }
    }

    internal static class ExceptionLoggingExtensions
    {
        public const string _errorLogged = "ErrorLogged";
        public static void MarkAsLogged(this Exception e)
        {
            e.Data.Add(_errorLogged, true);
        }
        public static bool IsLogged(this Exception e)
        {
            return e.Data.Contains(_errorLogged);
        }
    }
}

