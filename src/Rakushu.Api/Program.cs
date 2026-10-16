using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Extensions;
using Rakushu.Api.Middleware;
using Rakushu.Application.Abstractions.Infrastructure.ASR;
using Rakushu.Application.Extensions;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Extentions;
using Rakushu.Infrastructure.Extensions;
using Rakushu.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

/// CONFIG SOFTWARE LAYERS:
builder.Services.AddRakushuDomain();
builder.Services.AddRakushuPersistence(builder.Configuration);
builder.Services.AddRakushuInfrastructureForApi(builder.Configuration, builder.Environment);
builder.Services.AddRakushuApplication();
builder.Services.AddRakushuApi();

/// CONFIG MIDDLEWARES:
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

/// CONFIG SWAGGER DOCS:
builder.Services.AddSwaggerDocs();

var app = builder.Build();

/// USE GLOBAL EXCEPTION MIDDLEWARE:
app.UseExceptionHandler();

/// CORS:
app.UseCors("Frontend");

/// USE SWAGGER DOCS:
app.UseSwaggerDocs();

/// AUTHENTICATION & AUTHORIZATION:
app.UseAuthentication();
app.UseAuthorization();

/// MAP ENDPOINTS:
app.MapEndpoints();
app.MapGet("/", () => "Welcome to Rakushu Api").ExcludeFromDescription();

#region Test
//app.MapPost("/api/transcriptions/", async (
//	[FromBody] StartTranscriptionRequest request,
//		IAsrService transcriptionService,
//		CancellationToken cancellationToken
//	) =>
//{
//	var result = await transcriptionService.StartAsync(
//			request,
//			cancellationToken);

//	return Results.Accepted(
//		$"/api/transcriptions/{Uri.EscapeDataString(result.JobName)}",
//		result);
//})
//			.WithGroupName("storage")
//			.WithTags("TEST ASR")
//			.WithName("StartTranscription")
//			.WithSummary("Start an AWS Transcribe job")
//			.Produces<StartTranscriptionResult>(
//				StatusCodes.Status202Accepted)
//			.ProducesValidationProblem();

//// Get transcription job status
//app.MapGet("/api/transcriptions/{jobName}", async (
//	string jobName,
//		IAsrService transcriptionService,
//		CancellationToken cancellationToken
//	) =>
//{
//	var result = await transcriptionService.GetJobAsync(
//		   jobName,
//		   cancellationToken);

//	return result is null
//		? Results.NotFound()
//		: Results.Ok(result);
//})
//	.WithGroupName("storage")
//	.WithTags("TEST ASR")
//	.WithName("GetTranscription")
//	.WithSummary("Get transcription job status")
//	.Produces<TranscriptionJobResult>()
//	.Produces(StatusCodes.Status404NotFound);

//// Get transcription job status
//app.MapGet("/api/transcriptions/test-ast-output", async (
//	string storageKey,
//		IAsrOutputReader asrOutputReader,
//		CancellationToken cancellationToken
//	) =>
//{
//	var fullText = await asrOutputReader.ReadFullTextAsync(
//		   storageKey,
//		   cancellationToken);

//	var items = new List<TranscribeItemAsrReadDto>();

//	await foreach (var item in asrOutputReader.ReadItemsAsync(
//		storageKey,
//		cancellationToken))
//	{
//		items.Add(item);
//	}

//	var speakers = await asrOutputReader.ReadSpeakersAsync(
//		storageKey,
//		cancellationToken);

//	return Results.Ok(new TranscribeOutputDto(
//		FullText: fullText,
//		Speakers: speakers,
//		Items: items
//	));
//})
//	.WithGroupName("storage")
//	.WithTags("TEST ASR Output")
//	.WithName("GetTranscriptionOutput")
//	.WithSummary("Get transcription output")
//	.Produces<object>()
//	.Produces(StatusCodes.Status404NotFound);

//app.MapGet("/api/transcriptions/detail", async (
//		IVideoRepository videoRepository,
//		CancellationToken cancellationToken
//	) =>
//{
//	var video = await videoRepository.GetAllAsync(
//		cancellationToken);

//	return Results.Ok(video.ToList());
//})
//	.WithGroupName("learning")
//	.WithTags("TEST Transcript Detail ")
//	.WithName("GetTranscriptionDetail")
//	.WithSummary("Get transcription detail")
//	.Produces<Video>()
//	.Produces(StatusCodes.Status404NotFound);

#endregion

app.Run();
