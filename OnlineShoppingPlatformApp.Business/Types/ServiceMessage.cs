namespace OnlineShoppingPlatformApp.Business.Types
{
    public class ServiceMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class ServiceMessage<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}