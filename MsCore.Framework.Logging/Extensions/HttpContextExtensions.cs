using System.Text;
using Microsoft.AspNetCore.Http;

namespace MsCore.Framework.Logging.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task<string> ReadRequestBodyAsync(this HttpContext context)
        {
            string body = string.Empty;

            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
            {
                return body;
            }

            context.Request.EnableBuffering();

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
            }

            using var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);

            body = await reader.ReadToEndAsync();

            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
            }

            return body;
        }

        public static async Task<string> ReadResponseBodyAsync(this HttpContext context)
        {
            try
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(context.Response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                string body = await reader.ReadToEndAsync();

                context.Response.Body.Seek(0, SeekOrigin.Begin); // tekrar yazılabilir hale getir

                return body;
            }
            catch (Exception )
            {

                throw;
            }
    
        }
    }
}
