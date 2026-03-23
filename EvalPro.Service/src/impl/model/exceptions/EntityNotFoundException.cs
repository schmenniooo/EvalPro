namespace EvalProService.impl.exceptions;

/// <summary>
/// Exception thrown when an entity with the specified ID is not found.
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>Type name of the entity that was not found</summary>
    public string EntityType { get; }
    
    /// <summary>ID that was searched for</summary>
    public string EntityId { get; }

    /// <summary>
    /// Creates a new EntityNotFoundException.
    /// </summary>
    /// <param name="entityType">The type of entity (e.g. "Committee", "Examinee")</param>
    /// <param name="entityId">The ID that was not found</param>
    public EntityNotFoundException(string entityType, string entityId)
        : base($"{entityType} with ID '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <inheritdoc />
    public EntityNotFoundException(string entityType, string entityId, Exception innerException)
        : base($"{entityType} with ID '{entityId}' was not found.", innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
