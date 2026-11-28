namespace Rakushu.Application.Common.Pagination;

public class PaginatedList<T>
{
	public IReadOnlyCollection<T> Items { get; }
	public int PageNumber { get; }
	public int PageSize { get; }
	public int TotalPages { get; }
	public int TotalCount { get; }
	public bool HasPreviousPage => PageNumber > 1;
	public bool HasNextPage => PageNumber < TotalPages;

	public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
	{
		var safePageSize = pageSize > 0 ? pageSize : 1;
		PageNumber = pageNumber;
		PageSize = safePageSize;
		TotalPages = count > 0 ? (int)Math.Ceiling(count / (double)safePageSize) : 0;
		TotalCount = count;
		Items = items;
	}

	public static PaginatedList<T> Create(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
	{
		return new PaginatedList<T>(items, count, pageNumber, pageSize);
	}
}
