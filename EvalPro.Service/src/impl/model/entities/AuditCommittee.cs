namespace EvalProService.impl.model.entities;

public class AuditCommittee : BaseEntity
{
    public string Designation { get; set; }
    
    public string ApprenticeShip { get; set; }
    
    public List<DateTime> TestDates { get; set; }

    public Examinee? Examinee { get; set; }
    
    public AuditCommittee(string designation, string apprenticeShip, List<DateTime> testDates)
    {
        Designation = designation;
        ApprenticeShip = apprenticeShip;
        TestDates = testDates;
        
        // Examinee = new Examinee()
    }
}