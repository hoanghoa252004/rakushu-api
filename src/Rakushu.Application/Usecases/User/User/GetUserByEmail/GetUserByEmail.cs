using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.User.User.GetUserByEmail;

public sealed record UserLookupDto(string FullName, string Email, string? AvatarUrl, string? JlptLevel);

public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<UserLookupDto>>;

internal sealed class GetUserByEmailValidator : AbstractValidator<GetUserByEmailQuery>
{
	public GetUserByEmailValidator()
	{
		RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required");
	}
}

internal sealed class GetUserByEmailHandler(
	IUserRepository userRepository,
	IStorageService storageService)
	: IRequestHandler<GetUserByEmailQuery, Result<UserLookupDto>>
{
	public async Task<Result<UserLookupDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
	{
		var user = await userRepository.GetProfileByEmailAsync(request.Email.Trim(), cancellationToken);
		if (user is null)
			return Result.Failure<UserLookupDto>(UserErrors.NotFound);

		if (user.Status != UserStatus.Active)
			return Result.Failure<UserLookupDto>(UserErrors.NotActive);

		string? avatarUrl = null;
		if (!string.IsNullOrWhiteSpace(user.Profile?.AvatarKey))
			avatarUrl = await storageService.CreatePresignedReadUrlAsync(user.Profile!.AvatarKey!, cancellationToken);

		return Result.Success(new UserLookupDto(user.FullName, user.Email, avatarUrl, user.Profile?.Level?.Code));
	}
}
