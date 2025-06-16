namespace TaskManagerApi
{
    public class ErrorResponse
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
        public required int StatusCode { get; set; }

    }
}
