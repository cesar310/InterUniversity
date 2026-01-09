namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Exception thrown when authorization fails
/// </summary>
public sealed class AuthorizationException(string message, string errorCode = "UNAUTHORIZED")
    : DomainException(message)
{
    public override string ErrorCode => errorCode;
}
