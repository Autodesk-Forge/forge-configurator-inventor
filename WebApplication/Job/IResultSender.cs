using System.Threading.Tasks;

namespace WebApplication.Job
{
    /// <summary>
    /// Interface to send results back.
    /// </summary>
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