namespace StudentEnrollment.Domain.Exceptions;

/// <summary>
/// Exception thrown when a duplicate entity is detected
/// </summary>
public sealed class DuplicateException(string entityName, string fieldName, object value)
    : DomainException($"Ya existe un(a) {entityName} con {fieldName} '{value}'")
{
    public override string ErrorCode => "DUPLICATE_ENTITY";
    
    public string EntityName { get; } = entityName;
    public string FieldName { get; } = fieldName;
    public object Value { get; } = value;
}
