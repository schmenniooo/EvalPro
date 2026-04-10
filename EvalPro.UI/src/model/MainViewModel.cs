using System.Windows.Input;
using EvalProService.impl.model.entities;
using Service = EvalProService.impl.service.EvalProService;

namespace EvalProUI.model;

/// <summary>
/// Main ViewModel orchestrating navigation between views.
/// Owns the EvalProService instance and switches the active content view.
/// </summary>
public class MainViewModel : BaseViewModel
{
    private readonly Service _service;

    private BaseViewModel _currentView = null!;
    /// <summary>The currently displayed child view model.</summary>
    public BaseViewModel CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    private string _selectedNavItem = "Home";
    /// <summary>The currently selected navigation item name.</summary>
    public string SelectedNavItem
    {
        get => _selectedNavItem;
        set
        {
            if (SetProperty(ref _selectedNavItem, value))
                NavigateTo(value);
        }
    }

    /// <summary>Command to navigate to the home view.</summary>
    public ICommand NavigateHomeCommand { get; }
    /// <summary>Command to navigate to the committees view.</summary>
    public ICommand NavigateCommitteesCommand { get; }
    /// <summary>Command to navigate to the examinees view.</summary>
    public ICommand NavigateExamineesCommand { get; }

    /// <summary>Initializes the main view model with the given service.</summary>
    /// <param name="service">The backend service.</param>
    public MainViewModel(Service service)
    {
        _service = service;

        NavigateHomeCommand = new RelayCommand(() => NavigateTo("Home"));
        NavigateCommitteesCommand = new RelayCommand(() => NavigateTo("Prüfungsausschüsse"));
        NavigateExamineesCommand = new RelayCommand(() => NavigateTo("Prüflinge"));

        NavigateTo("Home");
    }

    /// <summary>Navigates to the view identified by the given name.</summary>
    /// <param name="viewName">Name of the target view.</param>
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

    /// <summary>Navigates to the detail view for the given examinee.</summary>
    /// <param name="examinee">The examinee to show details for.</param>
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
