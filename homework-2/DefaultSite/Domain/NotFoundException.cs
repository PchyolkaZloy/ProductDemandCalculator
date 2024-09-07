
public class NotFoundExceptionException : System.Exception
{
    public NotFoundExceptionException() { }
    public NotFoundExceptionException(string message) : base(message) { }
    public NotFoundExceptionException(string message, System.Exception inner) : base(message, inner) { }
}