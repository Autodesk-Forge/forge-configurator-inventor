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
        Task SendSuccessAsync();
        Task SendSuccessAsync(object arg0);
        Task SendSuccessAsync(object arg0, object arg1);
        Task SendSuccessAsync(object arg0, object arg1, object arg2);
        Task SendErrorAsync();
        Task SendErrorAsync(object arg0);
        Task SendErrorAsync(object arg0, object arg1);
        Task SendErrorAsync(object arg0, object arg1, object arg2);
    }
}