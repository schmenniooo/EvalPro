using EvalProService.impl.model.entities;

namespace EvalProService.impl.model.ratings;

public class BaseRating : BaseEntity
{
    public string FinalComment { get; set; }
    
    public Dictionary<string, int> PointsPerCriteria { get; set; }
    
    public Dictionary<string, string> CommentsPerCriteria { get; set; }

    public BaseRating(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
    {
        FinalComment = finalComment;
        PointsPerCriteria = pointsPerCriteria;
        CommentsPerCriteria = commentsPerCriteria;
    }
}