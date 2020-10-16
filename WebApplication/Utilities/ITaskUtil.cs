using System.Threading.Tasks;

namespace WebApplication.Utilities
{
    public interface ITaskUtil
    {
        void Sleep(int millis);
    }

    public class TaskUtil : ITaskUtil
    {
        public async void Sleep(int millis)
        {
            await Task.Delay(millis);
        }
    }
}