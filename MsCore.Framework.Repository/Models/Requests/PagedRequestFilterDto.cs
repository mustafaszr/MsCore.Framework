using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Repository.Models.Requests
{
    public class PagedRequestFilterDto
    {
        public string? ColumnName { get; set; }
        public string? Filter { get; set; }

        public PagedRequestFilterDto()
        {

        }

        public PagedRequestFilterDto(string columnName, string filter)
        {
            ColumnName = columnName;
            Filter = filter;
        }
    }
}
