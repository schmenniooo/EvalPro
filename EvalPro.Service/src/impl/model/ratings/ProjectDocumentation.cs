namespace EvalProService.impl.model.ratings;

public class ProjectDocumentation : BaseRating
{
    public ProjectDocumentation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}