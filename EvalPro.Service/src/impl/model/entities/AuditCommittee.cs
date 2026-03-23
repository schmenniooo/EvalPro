namespace EvalProService.impl.model.entities;

/// <summary>
/// Represents an audit committee that conducts IHK oral exams.
/// </summary>
public class AuditCommittee : BaseEntity
{
    /// <summary>Name/designation of the committee</summary>
    public string Designation { get; set; }
    
    /// <summary>The apprenticeship/profession this committee examines</summary>
    public string ApprenticeShip { get; set; }
    
    /// <summary>Scheduled examination dates</summary>
    public List<DateTime> TestDates { get; set; }

    /// <summary>The examinee assigned to this committee, or null</summary>
    public Examinee? Examinee { get; set; }

    /// <summary>
    /// Creates a new audit committee.
    /// </summary>
    public AuditCommittee(string designation, string apprenticeShip, List<DateTime> testDates)
    {
        Designation = designation;
        ApprenticeShip = apprenticeShip;
        TestDates = testDates;
    }
}