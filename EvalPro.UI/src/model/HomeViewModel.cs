using EvalProService.impl.service;

namespace EvalProUI.model;

/// <summary>
/// Dashboard ViewModel showing summary statistics.
/// </summary>
public class HomeViewModel : BaseViewModel
{
    private readonly EvalProService _service;

    public int CommitteeCount => _service.GetAllCommittees().Count;
    public int ExamineeCount => _service.GetAllExaminees().Count;
    public int AssignedCount => _service.GetAllCommittees().Count(c => c.Examinee != null);
    public int UnassignedExamineeCount => _service.GetAllExaminees()
        .Count(e => _service.GetCommitteeForExaminee(e) == null);

    public HomeViewModel(EvalProService service)
    {
        _service = service;
    }

    public void Refresh()
    {
        OnPropertyChanged(nameof(CommitteeCount));
        OnPropertyChanged(nameof(ExamineeCount));
        OnPropertyChanged(nameof(AssignedCount));
        OnPropertyChanged(nameof(UnassignedExamineeCount));
    }
}
