using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Models.Responses
{
    public class MsApiResponse<T> where T : class
    {
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public MsErrorDto? Error { get; set; }
        public string? Message { get; set; }
    }

    public class MsApiResponse : MsApiResponse<NoContentDto>
    {

    }
}
