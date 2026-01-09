namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found
/// </summary>
public sealed class NotFoundException(string message, string errorCode = "NOT_FOUND")
    : DomainException(message)
{
    public override string ErrorCode => errorCode;
}
