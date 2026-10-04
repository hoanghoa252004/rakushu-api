using Rakushu.Application.Extensions;
using Rakushu.Domain.Extentions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Extensions;
using Rakushu.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRakushuDomain();
builder.Services.AddRakushuPersistence(builder.Configuration);
builder.Services.AddRakushuInfrastructureForWorker(builder.Configuration, builder.Environment);
builder.Services.AddRakushuApplication();
builder.Services.AddRakushuWorkerServices();

var host = builder.Build();
host.Run();