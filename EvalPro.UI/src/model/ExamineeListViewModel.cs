using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EvalProService.impl.model.entities;
using Service = EvalProService.impl.service.EvalProService;

namespace EvalProUI.model;

/// <summary>
/// ViewModel for the Examinee list view with CRUD operations.
/// </summary>
public class ExamineeListViewModel : BaseViewModel
{
    private readonly Service _service;
    private readonly Action<Examinee> _navigateToDetail;

    /// <summary>Observable collection of all examinees.</summary>
    public ObservableCollection<Examinee> Examinees { get; } = new();

    private Examinee? _selectedExaminee;
    /// <summary>The currently selected examinee.</summary>
    public Examinee? SelectedExaminee
    {
        get => _selectedExaminee;
        set
        {
            if (SetProperty(ref _selectedExaminee, value))
            {
                if (value != null)
                {
                    FormName = value.Name;
                    FormCompany = value.Company;
                    FormContactPerson = value.ContactPerson;
                    FormProjectTitle = value.ProjectTitle;
                    IsEditing = true;
                }
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(CanViewDetail));
            }
        }
    }

    // --- Form Fields ---
    private string _formName = "";
    /// <summary>Form field for the examinee name.</summary>
    public string FormName
    {
        get => _formName;
        set => SetProperty(ref _formName, value);
    }

    private string _formCompany = "";
    /// <summary>Form field for the examinee company.</summary>
    public string FormCompany
    {
        get => _formCompany;
        set => SetProperty(ref _formCompany, value);
    }

    private string _formContactPerson = "";
    /// <summary>Form field for the contact person.</summary>
    public string FormContactPerson
    {
        get => _formContactPerson;
        set => SetProperty(ref _formContactPerson, value);
    }

    private string _formProjectTitle = "";
    /// <summary>Form field for the project title.</summary>
    public string FormProjectTitle
    {
        get => _formProjectTitle;
        set => SetProperty(ref _formProjectTitle, value);
    }

    private bool _isEditing;
    /// <summary>Whether the form is in edit mode (vs. create mode).</summary>
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    /// <summary>Whether an examinee is selected and can be deleted.</summary>
    public bool CanDelete => SelectedExaminee != null;
    /// <summary>Whether an examinee is selected and its details can be viewed.</summary>
    public bool CanViewDetail => SelectedExaminee != null;

    // --- Commands ---
    /// <summary>Command to save the current form (create or update).</summary>
    public ICommand SaveCommand { get; }
    /// <summary>Command to delete the selected examinee.</summary>
    public ICommand DeleteCommand { get; }
    /// <summary>Command to start creating a new examinee.</summary>
    public ICommand NewCommand { get; }
    /// <summary>Command to cancel the current form edit.</summary>
    public ICommand CancelCommand { get; }
    /// <summary>Command to navigate to the detail view for the selected examinee.</summary>
    public ICommand ViewDetailCommand { get; }

    /// <summary>Initializes the examinee list view model.</summary>
    /// <param name="service">The backend service.</param>
    /// <param name="navigateToDetail">Callback to navigate to the examinee detail view.</param>
    public ExamineeListViewModel(Service service, Action<Examinee> navigateToDetail)
    {
        _service = service;
        _navigateToDetail = navigateToDetail;
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete, () => CanDelete);
        NewCommand = new RelayCommand(NewExaminee);
        CancelCommand = new RelayCommand(Cancel);
        ViewDetailCommand = new RelayCommand(ViewDetail, () => CanViewDetail);
        LoadData();
    }

    /// <summary>Reloads all examinees from the service.</summary>
    public void LoadData()
    {
        Examinees.Clear();
        foreach (var e in _service.GetAllExaminees())
            Examinees.Add(e);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(FormName))
        {
            MessageBox.Show("Name darf nicht leer sein.", "Validierung", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsEditing && SelectedExaminee != null)
        {
            _service.UpdateExaminee(SelectedExaminee,
                name: FormName,
                company: FormCompany,
                contactPerson: FormContactPerson,
                projectTitle: FormProjectTitle);
        }
        else
        {
            _service.AddExaminee(FormName, FormCompany, FormContactPerson, FormProjectTitle);
        }

        ClearForm();
        LoadData();
    }

    private void Delete()
    {
        if (SelectedExaminee == null) return;

        var result = MessageBox.Show(
            $"Prüfling \"{SelectedExaminee.Name}\" wirklich löschen?",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _service.RemoveExaminee(SelectedExaminee);
            ClearForm();
            LoadData();
        }
    }

    private void NewExaminee()
    {
        ClearForm();
    }

    private void Cancel()
    {
        ClearForm();
    }

    private void ViewDetail()
    {
        if (SelectedExaminee != null)
            _navigateToDetail(SelectedExaminee);
    }

    private void ClearForm()
    {
        FormName = "";
        FormCompany = "";
        FormContactPerson = "";
        FormProjectTitle = "";
        IsEditing = false;
        SelectedExaminee = null;
    }
}
