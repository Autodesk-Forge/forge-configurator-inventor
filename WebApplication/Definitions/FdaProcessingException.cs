using System;

namespace WebApplication.Definitions
{
    /// <summary>
    /// Exception for FDA processing failures.
    /// Contains URL to processing report.
    /// </summary>
    public class FdaProcessingException : ApplicationException
    {
        public string ReportUrl { get; }

        public FdaProcessingException(string message, string reportUrl) : base(message)
        {
            ReportUrl = reportUrl;
        }
    }
}
