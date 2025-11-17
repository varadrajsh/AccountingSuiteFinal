using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSuite.Models.Common
{
    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            PageIndex = pageIndex < 1 ? 1 : pageIndex;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public int StartRecord => (PageIndex - 1) * PageSize + 1;
        public int EndRecord => Math.Min(PageIndex * PageSize, TotalCount);

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
