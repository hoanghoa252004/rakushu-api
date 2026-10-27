using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public sealed class SavedItem : AggregateRoot<SavedItemId>
{
	public UserId UserId { get; private set; } = null!;
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;

	// Vocabulary Snapshot
	public string Expression { get; private set; } = null!;
	public string? Reading { get; private set; }
	public string Meaning { get; private set; } = null!;

	public DateTimeOffset SavedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;
	public SrsSchedule.SrsSchedule? Schedule { get; private set; }

	private readonly List<SrsItemDetail.SrsItemDetail> _details = [];
	public IReadOnlyCollection<SrsItemDetail.SrsItemDetail> Details => _details.AsReadOnly();

	private SavedItem() { }
	private SavedItem(SavedItemId id) : base(id) { }

	public static Result<SavedItem> Create(
		UserId userId,
		KnowledgeMeaningId knowledgeMeaningId,
		string expression,
		string? reading,
		string meaning,
		DateTimeOffset now)
	{
		var validation = Validate(userId, knowledgeMeaningId, expression, meaning);
		if (validation.IsFailure)
			return Result.Failure<SavedItem>(validation.Error);

		return Result.Success(new SavedItem(SavedItemId.Create())
		{
			UserId = userId,
			KnowledgeMeaningId = knowledgeMeaningId,
			Expression = expression.Trim(),
			Reading = string.IsNullOrWhiteSpace(reading) ? null : reading.Trim(),
			Meaning = meaning.Trim(),
			SavedAt = now,
			CreatedAt = now
		});
	}

	private static Result Validate(
		UserId userId,
		KnowledgeMeaningId knowledgeMeaningId,
		string expression,
		string meaning)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure(SavedItemErrors.InvalidUserId);

		if (knowledgeMeaningId is null || knowledgeMeaningId.Value == Guid.Empty)
			return Result.Failure(SavedItemErrors.InvalidMeaningId);

		if (string.IsNullOrWhiteSpace(expression))
			return Result.Failure(SavedItemErrors.EmptyExpression);

		if (string.IsNullOrWhiteSpace(meaning))
			return Result.Failure(SavedItemErrors.EmptyMeaning);

		return Result.Success();
	}
}
