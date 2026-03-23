using EvalProService.impl.model.ratings;

namespace EvalProService.impl.model.entities;

/// <summary>
/// Represents a candidate being examined in an IHK oral exam.
/// </summary>
public class Examinee : BaseEntity
{
    /// <summary>Full name of the examinee</summary>
    public string Name { get; set; }
    
    /// <summary>Company the examinee is employed at</summary>
    public string Company { get; set; }
    
    /// <summary>Contact person at the company</summary>
    public string ContactPerson { get; set; }
    
    /// <summary>Title of the examinee's project</summary>
    public string ProjectTitle { get; set; }

    /// <summary>Rating for the project documentation, or null if not yet graded</summary>
    public ProjectDocumentation? ProjectDocumentation { get; set; }
    
    /// <summary>Rating for the project presentation, or null if not yet graded</summary>
    public ProjectPresentation? ProjectPresentation { get; set; }
    
    /// <summary>Rating for the technical conversation, or null if not yet graded</summary>
    public TechConversation? TechConversation { get; set; }
    
    /// <summary>Supplementary examination details, or null if not applicable</summary>
    public SupplementaryExamination? SupplementaryExamination { get; set; }

    /// <summary>
    /// Creates a new examinee with the given details.
    /// </summary>
    public Examinee(string name, string company, string contactPerson, string projectTitle) {
        Name = name;
        Company = company;
        ContactPerson = contactPerson;
        ProjectTitle = projectTitle;
    }
    
}