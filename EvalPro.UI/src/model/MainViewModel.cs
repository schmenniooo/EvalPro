using System.Windows.Input;
using EvalProService.impl.model.entities;
using EvalProService.impl.service;

namespace EvalProUI.model;

/// <summary>
/// Main ViewModel orchestrating navigation between views.
/// Owns the EvalProService instance and switches the active content view.
/// </summary>
public class MainViewModel : BaseViewModel
{
    private readonly EvalProService _service;

    private BaseViewModel _currentView = null!;
    public BaseViewModel CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    private string _selectedNavItem = "Home";
    public string SelectedNavItem
    {
        get => _selectedNavItem;
        set
        {
            if (SetProperty(ref _selectedNavItem, value))
                NavigateTo(value);
        }
    }

    // Navigation commands
    public ICommand NavigateHomeCommand { get; }
    public ICommand NavigateCommitteesCommand { get; }
    public ICommand NavigateExamineesCommand { get; }

    public MainViewModel(EvalProService service)
    {
        _service = service;

        NavigateHomeCommand = new RelayCommand(() => NavigateTo("Home"));
        NavigateCommitteesCommand = new RelayCommand(() => NavigateTo("Prüfungsausschüsse"));
        NavigateExamineesCommand = new RelayCommand(() => NavigateTo("Prüflinge"));

        NavigateTo("Home");
    }

    public void NavigateTo(string viewName)
    {
        _selectedNavItem = viewName;
        OnPropertyChanged(nameof(SelectedNavItem));

        CurrentView = viewName switch
        {
            "Home" => CreateHomeViewModel(),
            "Prüfungsausschüsse" => CreateCommitteeViewModel(),
            "Prüflinge" => CreateExamineeViewModel(),
            _ => CreateHomeViewModel()
        };
    }

    public void NavigateToExamineeDetail(Examinee examinee)
    {
        CurrentView = new ExamineeDetailViewModel(_service, examinee, () => NavigateTo("Prüflinge"));
    }

    private HomeViewModel CreateHomeViewModel()
    {
        return new HomeViewModel(_service);
    }

    private CommitteeListViewModel CreateCommitteeViewModel()
    {
        return new CommitteeListViewModel(_service);
    }

    private ExamineeListViewModel CreateExamineeViewModel()
    {
        return new ExamineeListViewModel(_service, NavigateToExamineeDetail);
    }
}
