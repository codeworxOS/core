namespace Codeworx.Extensions.DependencyInjection
{
    public interface IServiceLifecycle
    {
        void Remove(object instance);
    }
}