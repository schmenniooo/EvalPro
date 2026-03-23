using EvalProService.impl.model.entities;

namespace EvalProService.impl.model.ratings;

public class SupplementaryExamination : BaseEntity
{
    public string ChosenTestArea { get; set; }
    public int Points { get; set; }
    public List<string> Questions { get; set; }

    public SupplementaryExamination(string chosenTestArea, int points, List<string> questions)
    {
        ChosenTestArea = chosenTestArea;
        Points = points;
        Questions = questions;
    }
}