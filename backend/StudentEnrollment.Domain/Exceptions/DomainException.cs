namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-specific exceptions
/// </summary>
public abstract class DomainException(string message) : Exception(message)
{
    public virtual string ErrorCode => "DOMAIN_ERROR";
}
