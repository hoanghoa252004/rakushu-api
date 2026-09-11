using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Email;

internal class AwsSesService : IEmailService
{
	private readonly IAmazonSimpleEmailService _ses;
	private readonly AwsSettings _emailSettings;

	public AwsSesService(IAmazonSimpleEmailService ses, IOptions<AwsSettings> options)
	{
		_ses = ses;
		_emailSettings = options.Value;
	}

	public async Task<Result> SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
	{

		var request = new SendEmailRequest
		{
			Source = _emailSettings.SenderEmail,
			Destination = new Destination
			{
				ToAddresses = [toEmail]
			},
			Message = new Message
			{
				Subject = new Content(subject),
				Body = new Body
				{
					Html = new Content(body)
				}
			}
		};

		var response = await _ses.SendEmailAsync(request, cancellationToken);

		if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
			return Result.Failure(CommonError.FailedSendEmail);

		return Result.Success();
	}
}
