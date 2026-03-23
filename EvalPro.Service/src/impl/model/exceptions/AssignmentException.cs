namespace EvalProService.impl.exceptions;

/// <summary>
/// Exception thrown when an entity assignment operation fails.
/// </summary>
public class AssignmentException : Exception
{
    /// <summary>Type of the source entity in the failed assignment</summary>
    public string SourceEntityType { get; }
    
    /// <summary>ID of the source entity</summary>
    public string SourceEntityId { get; }
    
    /// <summary>Type of the target entity in the failed assignment</summary>
    public string TargetEntityType { get; }
    
    /// <summary>ID of the target entity</summary>
    public string TargetEntityId { get; }

    /// <summary>
    /// Creates a new AssignmentException with full context about the failed assignment.
    /// </summary>
    public AssignmentException(string sourceEntityType, string sourceEntityId, string targetEntityType, string targetEntityId, string reason)
        : base($"Failed to assign {sourceEntityType} '{sourceEntityId}' to {targetEntityType} '{targetEntityId}': {reason}")
    {
        SourceEntityType = sourceEntityType;
        SourceEntityId = sourceEntityId;
        TargetEntityType = targetEntityType;
        TargetEntityId = targetEntityId;
    }

    /// <summary>
    /// Creates a new AssignmentException with a simple message.
    /// </summary>
    public AssignmentException(string message) : base(message)
    {
        SourceEntityType = string.Empty;
        SourceEntityId = string.Empty;
        TargetEntityType = string.Empty;
        TargetEntityId = string.Empty;
    }
}
