using Rakushu.Application.Abstractions.Infrastructure.Realtime;
using Rakushu.Application.Extensions;
using Rakushu.Domain.Extentions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Extensions;
using Rakushu.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRakushuDomain();
builder.Services.AddRakushuPersistence(builder.Configuration);
builder.Services.AddRakushuInfrastructureForWorker(builder.Configuration, builder.Environment);
builder.Services.AddRakushuApplication();
builder.Services.AddRakushuWorkerServices();
builder.Services.AddScoped<IShadowingRealtimeNotifier, SignalRShadowingRealtimeNotifierForWorker>();

var host = builder.Build();
host.Run();

internal sealed class SignalRShadowingRealtimeNotifierForWorker : IShadowingRealtimeNotifier
{
	public Task NotifyRoleLineCompletedAsync(RoleLineCompletedMessage message, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}