namespace EvalProService.impl.model.entities;

public class Examinee : BaseEntity
{
    public string Name { get; set; }
    
    public string Company { get; set; }
    
    public string ContactPerson { get; set; }
    
    public string ProjectTitle { get; set; }
    
    public ProjectDocumentation ProjectDocumentation { get; set; }
    
    public ProjectPresentation ProjectPresentation { get; set; }
    
    public TechConversation TechConversation { get; set; }
    
    public SupplementaryExamination SupplementaryExamination { get; set; }

    public Examinee(
        string name, 
        string company, 
        string contactPerson, 
        string projectTitle
    ) {
        Name = name;
        Company = company;
        ContactPerson = contactPerson;
        ProjectTitle = projectTitle;
    }
    
    public void SetProjectDocumentation(ProjectDocumentation documentation)
    {
        ProjectDocumentation = documentation;
    }

    public void SetProjectPresentation(ProjectPresentation presentation)
    {
        ProjectPresentation = presentation;
    }

    public void SetTechConversation(TechConversation techConversation)
    {
        TechConversation = techConversation;
    }

    public void SetSupplementaryExamination(SupplementaryExamination supplementaryExamination)
    {
        SupplementaryExamination = supplementaryExamination;
    }
    
}