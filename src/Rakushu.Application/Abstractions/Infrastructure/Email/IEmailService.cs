using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Email;

public interface IEmailService
{
	Task<Result> SendAsync(string toEmail, string subject, string body, CancellationToken cancellation);
}
