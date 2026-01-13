namespace EvalProService.impl.model.rating;

public class BaseRating
{
    public string FinalComment { get; set; }
    
    public Dictionary<string, int> PointsPerCriteria { get; set; }
    
    public Dictionary<string, string> CommentsPerCriteria { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ModifiedAt { get; set; }
    
    public BaseRating(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
    {
        FinalComment = finalComment;
        PointsPerCriteria = pointsPerCriteria;
        CommentsPerCriteria = commentsPerCriteria;
        CreatedAt = DateTime.Now;
        ModifiedAt = DateTime.Now;
    }
}