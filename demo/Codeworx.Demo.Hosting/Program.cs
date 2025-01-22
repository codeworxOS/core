////using Microsoft.AspNetCore.Builder;

////var builder = WebApplication.CreateBuilder(args);
////await builder.HostAsync();

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;

var builder = WebHost.CreateDefaultBuilder();
await builder.HostAsync();