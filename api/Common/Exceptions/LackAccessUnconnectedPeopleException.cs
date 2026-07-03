namespace Common.Exceptions
{
    public class LackAccessUnconnectedPeopleException : AppException
    {
        public LackAccessUnconnectedPeopleException()
       : base(ApiResultStatusCode.Unreachable, System.Net.HttpStatusCode.NotAcceptable)
        {
        }

        public LackAccessUnconnectedPeopleException(string message)
            : base(ApiResultStatusCode.Unreachable, message, System.Net.HttpStatusCode.NotAcceptable)
        {
        }
    }
}
