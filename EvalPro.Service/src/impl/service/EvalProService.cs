using EvalProService.api;
using EvalProService.impl.model;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;
using EvalProService.impl.persistency;
using EvalProService.impl.persistency.autoSaver;

namespace EvalProService.impl.service;

public class EvalProService : IEvalProServiceApi
{
    private readonly ServiceData _data;

    public EvalProService()
    {
        _data = new ServiceData();
        new AutoDataSaver(_data).StartAutoSaveTimer();
    }
    
    public List<AuditCommittee> GetCommitteesList()
    {
        return _data.CommitteesList;
    }

    public List<Examinee> GetExamineesList()
    {
        return _data.ExamineesList;
    }

    public List<ProjectDocumentation> GetProjectDocumentationList()
    {
        return _data.ProjectDocumentationList;
    }

    public List<ProjectPresentation> GetProjectPresentationList()
    {
        return _data.ProjectPresentationList;
    }

    public List<TechConversation> GetProjectTechConversationList()
    {
        return _data.ProjectTechConversationList;
    }

    public List<SupplementaryExamination> GetSupplementaryExaminationList()
    {
        return _data.SupplementaryExaminationList;
    }

    public AuditCommittee AddCommittee(string designation, string apprenticeShip, List<DateTime> testDates)
    {
        var committee = new AuditCommittee(designation, apprenticeShip, testDates);
        _data.CommitteesList.Add(committee);
        return committee;
    }

    public Examinee AddExaminee(string name, string company, string contactPerson, string projectTitle)
    {
        var examinee = new Examinee(name, company, contactPerson, projectTitle);
        _data.ExamineesList.Add(examinee);
        return examinee;
    }
    
}