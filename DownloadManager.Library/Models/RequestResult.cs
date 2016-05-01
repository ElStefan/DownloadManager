namespace DownloadManager.Library.Models
{
    public class RequestResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class RequestResult<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}