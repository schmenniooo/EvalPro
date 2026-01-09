using EvalProService.impl.model.rating;

namespace EvalProService.impl.model;

public class ProjectPresentation : BaseRating
{
    public ProjectPresentation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}