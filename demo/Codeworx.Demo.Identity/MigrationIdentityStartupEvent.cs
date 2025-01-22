using System;
using System.Threading.Tasks;
using Codeworx.Hosting;
using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Demo.Identity
{
    public class MigrationIdentityStartupEvent : IStartupEvent
    {
        public MigrationIdentityStartupEvent(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task StartAsync()
        {
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();

                await ctx.Database.MigrateAsync();
                await ctx.DataMigrator().MigrateAsync();
            }
        }
    }
}