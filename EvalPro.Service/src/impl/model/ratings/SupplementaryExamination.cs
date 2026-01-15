namespace EvalProService.impl.model.ratings;

public class SupplementaryExamination
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public string ChosenTestArea { get; set; }
    public int Points { get; set; }
    public List<string> Questions { get; set; }

    public SupplementaryExamination(string chosenTestArea, int points,  List<string> questions)
    {
        ChosenTestArea = chosenTestArea;
        Points = points;
        Questions = questions;
    }
}