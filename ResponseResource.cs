namespace Html2Pdf
{
    public class ResponseResource
    {
        public ResponseCode Code { get; set; }

        public string Message { get; set; }

        public string Data { get; set; }
    }

    public enum ResponseCode : int
    {
        Success, Failure
    }
}