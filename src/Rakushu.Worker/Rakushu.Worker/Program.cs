using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Extensions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Extensions;
using Rakushu.Worker.Extensions;
using Rakushu.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRakushuPersistence(builder.Configuration);
builder.Services.AddRakushuInfrastructureForWorker(builder.Configuration);
builder.Services.AddRakushuApplication();
builder.Services.AddRakushuWorkerServices();

var host = builder.Build();
host.Run();
