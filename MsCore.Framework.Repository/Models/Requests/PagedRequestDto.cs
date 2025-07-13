using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Repository.Models.Requests
{
    public class PagedRequestDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortColumnName { get; set; }
        public bool? OrderByDescending { get; set; } = false;
        public List<PagedRequestFilterDto>? Filters { get; set; }

        public PagedRequestDto()
        {

        }

        public PagedRequestDto(int pageNumgber, int pageSize, string? sortColumnName = null, bool? orderByDescendin = null,List<PagedRequestFilterDto>? filters = null)
        {
            PageNumber = pageNumgber;
            PageSize = pageSize;
            SortColumnName = sortColumnName;
            OrderByDescending = orderByDescendin;
            Filters = filters;
        }
    }
}
