using System.Threading.Tasks;

namespace webapplication.Utilities
{
    public interface ITaskUtil
    {
        Task Sleep(int millis);
    }

    public class TaskUtil : ITaskUtil
    {
        public async Task Sleep(int millis)
        {
            await Task.Delay(millis);
        }
    }
}