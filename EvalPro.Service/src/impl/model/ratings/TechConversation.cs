namespace EvalProService.impl.model.ratings;

public class TechConversation : BaseRating
{
    public TechConversation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}