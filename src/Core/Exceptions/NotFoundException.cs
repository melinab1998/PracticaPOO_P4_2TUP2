namespace Core.Exceptions;

public class NotFoundException : Exception
{
    public string ErrorCode { get; private set; }
    public NotFoundException()
    : base()
    {
    }

    public NotFoundException(string message, string errorCode = "")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}
