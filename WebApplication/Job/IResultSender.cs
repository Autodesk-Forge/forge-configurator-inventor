using System.Threading.Tasks;

namespace WebApplication.Job
{
    /// <summary>
    /// Interface with callbacks to send results back.
    /// </summary>
    /// <remarks>
    /// Unfortunately SignalR disallow overloaded methods, so need to name them differently.
    /// </remarks>
    public interface IResultSender
    {
        Task SendSuccess0Async();
        Task SendSuccess1Async(object arg0);
        Task SendSuccess2Async(object arg0, object arg1);
        Task SendSuccess3Async(object arg0, object arg1, object arg2);
    }
}