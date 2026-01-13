namespace EvalProService.impl.model;

public class Examinee
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
        string projectTitle, 
        ProjectDocumentation projectDocumentation, 
        ProjectPresentation projectPresentation, 
        TechConversation techConversation,  
        SupplementaryExamination supplementaryExamination
    ) {
        Name = name;
        Company = company;
        ContactPerson = contactPerson;
        ProjectTitle = projectTitle;
        
        ProjectDocumentation = projectDocumentation;
        ProjectPresentation = projectPresentation;
        TechConversation = techConversation;
        SupplementaryExamination = supplementaryExamination;
    }
}