using EvalProService.impl.model.entities;

namespace EvalProService.impl.model.ratings;

/// <summary>
/// Represents the supplementary oral examination with free-form questions.
/// </summary>
public class SupplementaryExamination : BaseEntity
{
    /// <summary>The subject area chosen for the examination</summary>
    public string ChosenTestArea { get; set; }
    
    /// <summary>Total points awarded</summary>
    public int Points { get; set; }
    
    /// <summary>List of questions asked during the examination</summary>
    public List<string> Questions { get; set; }

    /// <summary>
    /// Creates a new supplementary examination record.
    /// </summary>
    public SupplementaryExamination(string chosenTestArea, int points, List<string> questions)
    {
        ChosenTestArea = chosenTestArea;
        Points = points;
        Questions = questions;
    }
}