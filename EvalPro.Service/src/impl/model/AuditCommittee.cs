namespace EvalProService.impl.model;

public class AuditCommittee
{
    public string Designation { get; set; }
    
    public string ApprenticeShip { get; set; }
    
    public List<DateTime> TestDates { get; set; }
    
    public AuditCommittee(string designation, string apprenticeShip, List<DateTime> testDates)
    {
        Designation = designation;
        ApprenticeShip = apprenticeShip;
        TestDates = testDates;
    }
}