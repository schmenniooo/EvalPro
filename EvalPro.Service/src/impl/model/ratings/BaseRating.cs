using EvalProService.impl.model.entities;

namespace EvalProService.impl.model.ratings;

/// <summary>
/// Base class for criteria-based ratings. Contains scoring and comments per criterion.
/// </summary>
public class BaseRating : BaseEntity
{
    /// <summary>Overall concluding comment for the rating</summary>
    public string FinalComment { get; set; }
    
    /// <summary>Points awarded per criterion (criterion name → points)</summary>
    public Dictionary<string, int> PointsPerCriteria { get; set; }
    
    /// <summary>Comments per criterion (criterion name → comment)</summary>
    public Dictionary<string, string> CommentsPerCriteria { get; set; }

    /// <summary>
    /// Creates a new rating with the given comment and per-criteria scores.
    /// </summary>
    public BaseRating(string finalComment, Dictionary<string, int> pointsPerCriteria, Dictionary<string, string> commentsPerCriteria)
    {
        FinalComment = finalComment;
        PointsPerCriteria = pointsPerCriteria;
        CommentsPerCriteria = commentsPerCriteria;
    }
}