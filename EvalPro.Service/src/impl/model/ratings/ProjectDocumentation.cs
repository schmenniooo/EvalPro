namespace EvalProService.impl.model.ratings;

/// <summary>
/// Rating for the project documentation component of the exam.
/// </summary>
public class ProjectDocumentation : BaseRating
{
    /// <inheritdoc />
    public ProjectDocumentation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}