namespace EvalProService.impl.model;

public class BaseEntity
{
    public string ID { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public BaseEntity()
    {
        ID = Guid.NewGuid().ToString();
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}