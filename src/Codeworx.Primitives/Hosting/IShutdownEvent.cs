using System.Threading.Tasks;

namespace Codeworx.Hosting
{
    public interface IShutdownEvent
    {
        Task ShutdownAsync();
    }
}