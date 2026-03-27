using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EvalProService.impl.model.entities;
using EvalProService.impl.service;

namespace EvalProUI.model;

/// <summary>
/// ViewModel for the Examinee list view with CRUD operations.
/// </summary>
public class ExamineeListViewModel : BaseViewModel
{
    private readonly EvalProService _service;
    private readonly Action<Examinee> _navigateToDetail;

    public ObservableCollection<Examinee> Examinees { get; } = new();

    private Examinee? _selectedExaminee;
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
    public string FormName
    {
        get => _formName;
        set => SetProperty(ref _formName, value);
    }

    private string _formCompany = "";
    public string FormCompany
    {
        get => _formCompany;
        set => SetProperty(ref _formCompany, value);
    }

    private string _formContactPerson = "";
    public string FormContactPerson
    {
        get => _formContactPerson;
        set => SetProperty(ref _formContactPerson, value);
    }

    private string _formProjectTitle = "";
    public string FormProjectTitle
    {
        get => _formProjectTitle;
        set => SetProperty(ref _formProjectTitle, value);
    }

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public bool CanDelete => SelectedExaminee != null;
    public bool CanViewDetail => SelectedExaminee != null;

    // --- Commands ---
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ViewDetailCommand { get; }

    public ExamineeListViewModel(EvalProService service, Action<Examinee> navigateToDetail)
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
