namespace EvalProService.impl.model.ratings;

public class ProjectPresentation : BaseRating
{
    public ProjectPresentation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}