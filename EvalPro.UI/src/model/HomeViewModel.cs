using Service = EvalProService.impl.service.EvalProService;

namespace EvalProUI.model;

/// <summary>
/// Dashboard ViewModel showing summary statistics.
/// </summary>
public class HomeViewModel : BaseViewModel
{
    private readonly Service _service;

    /// <summary>Total number of audit committees.</summary>
    public int CommitteeCount => _service.GetAllCommittees().Count;
    /// <summary>Total number of examinees.</summary>
    public int ExamineeCount => _service.GetAllExaminees().Count;
    /// <summary>Number of committees that have an examinee assigned.</summary>
    public int AssignedCount => _service.GetAllCommittees().Count(c => c.Examinee != null);
    /// <summary>Number of examinees not assigned to any committee.</summary>
    public int UnassignedExamineeCount => _service.GetAllExaminees()
        .Count(e => _service.GetCommitteeForExaminee(e) == null);

    /// <summary>Initializes the home view model with the given service.</summary>
    /// <param name="service">The backend service.</param>
    public HomeViewModel(Service service)
    {
        _service = service;
    }

    /// <summary>Refreshes all summary statistics.</summary>
    public void Refresh()
    {
        OnPropertyChanged(nameof(CommitteeCount));
        OnPropertyChanged(nameof(ExamineeCount));
        OnPropertyChanged(nameof(AssignedCount));
        OnPropertyChanged(nameof(UnassignedExamineeCount));
    }
}
