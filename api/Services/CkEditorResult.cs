using Common;

namespace Services
{
    public class CkEditorResult
    {
        public CkEditorResult(bool isSuccess, string link, ApiResultStatusCode statusCode)
        {
            IsSuccess = isSuccess;
            Link = link;
            StatusCode = statusCode;
        }
        public bool IsSuccess { get; set; }
        public string Link { get; set; }
        public ApiResultStatusCode StatusCode { get; set; }

        //public static ServiceResult Ok(string link)
        //{
        //    return new ServiceResult(true, link, ApiResultStatusCode.Success);
        //}
        public static CkEditorResult<TData> Ok<TData>(TData data, string message) where TData : class
        {
            return new CkEditorResult<TData>(true, message, ApiResultStatusCode.Success, data);
        }
        public static CkEditorResult<TData> Ok<TData>(TData data) where TData : class
        {
            return new CkEditorResult<TData>(true, "عملیات موفق بود", ApiResultStatusCode.Success, data);
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class IgnoreApiResultFilterAttribute : Attribute
    {
    }

    public class CkEditorResult<TData> : ServiceResult where TData : class
    {
        public CkEditorResult(bool isSuccess, string message, ApiResultStatusCode statusCode, TData data) : base(isSuccess, message, statusCode)
        {
            Link = data;
        }
        public TData Link { get; set; }
    }
}
