namespace EvalProService.impl.model.ratings;

/// <summary>
/// Rating for the project presentation component of the exam.
/// </summary>
public class ProjectPresentation : BaseRating
{
    /// <inheritdoc />
    public ProjectPresentation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}