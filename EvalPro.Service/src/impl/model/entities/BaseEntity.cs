namespace EvalProService.impl.model.entities;

/// <summary>
/// Base class for all entities. Provides a unique ID and timestamps.
/// </summary>
public class BaseEntity
{
    /// <summary>Unique identifier (GUID string)</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>When this entity was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>When this entity was last modified</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}