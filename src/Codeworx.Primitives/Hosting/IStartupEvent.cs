using System.Threading.Tasks;

namespace Codeworx.Hosting
{
    public interface IStartupEvent
    {
        Task StartAsync();
    }
}