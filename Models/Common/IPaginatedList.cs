namespace AccountingSuite.Models.Common
{
    public interface IPaginatedList
    {
        int PageIndex { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
