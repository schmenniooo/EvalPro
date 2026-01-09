namespace EvalProService.impl.model;

public class Examinee
{
    public string Name { get; set; }
    
    public string Company { get; set; }
    
    public string ContactPerson { get; set; }
    
    public string ProjectTitle { get; set; }

    public Examinee(string name, string company, string contactPerson, string projectTitle)
    {
        Name = name;
        Company = company;
        ContactPerson = contactPerson;
        ProjectTitle = projectTitle;
    }
}