namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Exception thrown when domain validation fails
/// </summary>
public sealed class DomainValidationException(string message, string errorCode = "VALIDATION_ERROR")
    : DomainException(message)
{
    public override string ErrorCode => errorCode;
}
