namespace JRpcMediator.Client.Models;

public class BatchResult
{
    public BatchResult(int id, object? result)
    {
        Id = id;
        Result = result;
    }

    public BatchResult(int id, Exception? exception)
    {
        Id = id;
        Exception = exception;
    }

    public int Id { get; set; }
    public object? Result { get; set; }
    public Exception? Exception { get; set; }
}