using MediatR;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.CreateSpeaker;

public sealed record CreateSpeakerCommand(
	string Code,
	string Name,
	string JapaneseName,
	string? AvatarKey,
	string? Description,
	string ThemeColor) : IRequest<Result<Guid>>;