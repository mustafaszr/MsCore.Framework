using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MsCore.Framework.Repository.Models
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T>? Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PagedResult()
        {
            
        }

        public PagedResult(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
        {
            Data = data is IReadOnlyList<T> readOnlyList ? readOnlyList : data.ToList().AsReadOnly();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < TotalPages;
        }
    }
}
