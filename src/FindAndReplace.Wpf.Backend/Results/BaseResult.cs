using System;

namespace FindAndReplace.Wpf.Backend.Results
{
    public abstract class BaseResult<TPayload>
    {
        public string ErrorMessage { get; protected set; }
        public Exception Exception { get; protected set; }
        public bool IsSuccessful { get; protected set; }
        public string Path { get; protected set; }
        public TPayload Payload { get; protected set; }

        protected BaseResult()
        {

        }

        public static TResult CreateSuccess<TResult>(string path, TPayload payload)
            where TResult : BaseResult<TPayload>, new()
        {
            var br = new TResult
            {
                ErrorMessage = String.Empty,
                Exception = null,
                IsSuccessful = true,
                Path = path,
                Payload = payload
            };
            return br;
        }

        public static TResult CreateFailure<TResult>(string path, string errorMessage)
            where TResult : BaseResult<TPayload>, new()
        {
            return CreateFailure<TResult>(path, errorMessage, null);
        }

        public static TResult CreateFailure<TResult>(string path, Exception exception)
            where TResult : BaseResult<TPayload>, new()
        {
            return CreateFailure<TResult>(path, String.Empty, exception);
        }

        public static TResult CreateFailure<TResult>(string path, string errorMessage, Exception exception)
            where TResult : BaseResult<TPayload>, new()
        {
            var br = new TResult
            {
                ErrorMessage = errorMessage,
                Exception = exception,
                IsSuccessful = false,
                Path = path,
                Payload = default
            };
            return br;
        }

    }
}
