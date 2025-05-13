using Contract.Shared.Constants;

namespace Contract.Shared;

public class PaginatedList<T>(List<T> items, int count, int pageIndex, int pageSize)
{
    public List<T> Items { get; private set; } = items;
    public int PageIndex { get; private set; } = pageIndex;
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);
    public int TotalCount { get; } = count;
    public int PageSize { get; private set; } = pageSize;
}