using EvalProService.impl.model.ratings;

namespace EvalProService.impl.model.entities;

public class Examinee : BaseEntity
{
    public string Name { get; set; }
    
    public string Company { get; set; }
    
    public string ContactPerson { get; set; }
    
    public string ProjectTitle { get; set; }

    public string? ProjectDocumentationId { get; set; }
    
    public string? ProjectPresentationId { get; set; }
    
    public string? TechConversationId { get; set; }
    
    public string? SupplementaryExaminationId { get; set; }

    public Examinee(string name, string company, string contactPerson, string projectTitle) {
        Name = name;
        Company = company;
        ContactPerson = contactPerson;
        ProjectTitle = projectTitle;
    }
    
}