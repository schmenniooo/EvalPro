namespace EvalProService.impl.exceptions;

/// <summary>
/// Exception thrown when an assignment operation fails
/// </summary>
public class AssignmentException : Exception
{
    public string SourceEntityType { get; }
    public string SourceEntityId { get; }
    public string TargetEntityType { get; }
    public string TargetEntityId { get; }

    public AssignmentException(string sourceEntityType, string sourceEntityId, string targetEntityType, string targetEntityId, string reason)
        : base($"Failed to assign {sourceEntityType} '{sourceEntityId}' to {targetEntityType} '{targetEntityId}': {reason}")
    {
        SourceEntityType = sourceEntityType;
        SourceEntityId = sourceEntityId;
        TargetEntityType = targetEntityType;
        TargetEntityId = targetEntityId;
    }

    public AssignmentException(string message) : base(message)
    {
        SourceEntityType = string.Empty;
        SourceEntityId = string.Empty;
        TargetEntityType = string.Empty;
        TargetEntityId = string.Empty;
    }
}
