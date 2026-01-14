using EvalProService.impl.model.rating;

namespace EvalProService.impl.model;

public class TechConversation : BaseRating
{
    public TechConversation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}