using Spoksy.Domain.Entities;

namespace Spoksy.Domain.ValueObjects
{
    public class PagedResult<T> where T : Entity
    {
        public IEnumerable<T> Items { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalItems { get; }
        public int TotalPages { get; }

        public PagedResult(IEnumerable<T> items, int currentPage, int pageSize, int totalItems)
        {
            Items = items;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = (totalItems + pageSize - 1) / pageSize;
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
} 