namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public sealed class AuthenticationException(string message, string errorCode = "AUTHENTICATION_FAILED")
    : DomainException(message)
{
    public override string ErrorCode => errorCode;
}
