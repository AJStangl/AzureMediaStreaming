using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureMediaStreaming.ActionResults
{
    public class VideoResponse : ObjectResult
    {
        public VideoResponse(object value, int statusCode) : base(value)
        {
            Value = value;
            StatusCode = statusCode;
        }
    }

    public class VideoResultError
    {
        public string ErrorMessage { get; set; }
        public string TraceIdentifier { get; set; }
        public ErrorType? ErrorType { get; set; }


        public static VideoResultError CreateInstance(string errorMessage, HttpContext httpContext,
            ErrorType? errorType)
        {
            return new VideoResultError
            {
                ErrorMessage = errorMessage,
                TraceIdentifier = httpContext.TraceIdentifier,
                ErrorType = errorType
            };
        }
    }

    public enum ErrorType
    {
        InternalServer,
        NotFound
    }
}