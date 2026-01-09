namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public sealed class BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
    : DomainException(message)
{
    public override string ErrorCode => errorCode;
}
