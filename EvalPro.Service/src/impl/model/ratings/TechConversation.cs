namespace EvalProService.impl.model.ratings;

/// <summary>
/// Rating for the technical conversation component of the exam.
/// </summary>
public class TechConversation : BaseRating
{
    /// <inheritdoc />
    public TechConversation(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
        : base(finalComment, pointsPerCriteria, commentsPerCriteria)
    {
    }
}