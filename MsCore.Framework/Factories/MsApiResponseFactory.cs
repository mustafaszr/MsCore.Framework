using System.Net;
using MsCore.Framework.Models.Responses;

namespace MsCore.Framework.Factories
{
    public static class MsApiResponseFactory
    {
        public static MsApiResponse Success(HttpStatusCode statusCode, string message)
        {
            return new MsApiResponse { StatusCode = statusCode, IsSuccessful = true, Message = message };
        }

        public static MsApiResponse Fail(MsErrorDto errorDto, HttpStatusCode statusCode)
        {
            return new MsApiResponse { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static MsApiResponse Fail(string error, HttpStatusCode statusCode, bool isShow = true)
        {
            var errorDto = new MsErrorDto(error, isShow);
            return new MsApiResponse { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static MsApiResponse<T> Success<T>(T data, HttpStatusCode statusCode, string message) where T : class
        {
            return new MsApiResponse<T> { Data = data, StatusCode = statusCode, IsSuccessful = true, Message = message };
        }

        public static MsApiResponse<T> Fail<T>(MsErrorDto errorDto, HttpStatusCode statusCode) where T : class
        {
            return new MsApiResponse<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static MsApiResponse<T> Fail<T>(string error, HttpStatusCode statusCode, bool isShow = true) where T : class
        {
            var errorDto = new MsErrorDto(error, isShow);
            return new MsApiResponse<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }
    }
}
