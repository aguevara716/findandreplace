using System;
using System.Text;

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

        public string GetErrorText()
        {
            if (IsSuccessful)
                return string.Empty;

            var errorTextBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(ErrorMessage))
                errorTextBuilder.Append(ErrorMessage);
            if (!string.IsNullOrEmpty(ErrorMessage) && Exception != null)
                errorTextBuilder.AppendLine();
            if (Exception != null)
                errorTextBuilder.Append(Exception.Message);
            var errorText = errorTextBuilder.ToString();
            return errorText;
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
