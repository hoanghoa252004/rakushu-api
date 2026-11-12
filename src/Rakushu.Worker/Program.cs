using Rakushu.Application.Abstractions.Infrastructure.Realtime;
using Rakushu.Application.Extensions;
using Rakushu.Domain.Extentions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Extensions;
using Rakushu.Worker;

var builder = Host.CreateApplicationBuilder(args);


// Log runtime environment and Azure Speech configuration
var configuration = builder.Configuration;
var environment = builder.Environment;

var speechKey = configuration["AwsSettings:AzureSpeechKey"];
var speechRegion = configuration["AwsSettings:AzureSpeechRegion"];

Console.WriteLine("========== RAKUSHU WORKER CONFIGURATION ==========");
Console.WriteLine($"Environment       : {environment.EnvironmentName}");
Console.WriteLine($"Application Name  : {environment.ApplicationName}");
Console.WriteLine($"Content Root      : {environment.ContentRootPath}");
Console.WriteLine($"Speech Key Configured : {!string.IsNullOrWhiteSpace(speechKey)}");
Console.WriteLine($"Speech Key Length     : {speechKey?.Length ?? 0}");
Console.WriteLine($"Speech Region         : {speechRegion}");
Console.WriteLine("==================================================");

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