using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EvalProService.impl.model.entities;
using Service = EvalProService.impl.service.EvalProService;

namespace EvalProUI.model;

/// <summary>
/// ViewModel for the Audit Committee list view with CRUD operations.
/// </summary>
public class CommitteeListViewModel : BaseViewModel
{
    private readonly Service _service;

    /// <summary>Observable collection of all audit committees.</summary>
    public ObservableCollection<AuditCommittee> Committees { get; } = new();

    private AuditCommittee? _selectedCommittee;
    /// <summary>The currently selected audit committee.</summary>
    public AuditCommittee? SelectedCommittee
    {
        get => _selectedCommittee;
        set
        {
            if (SetProperty(ref _selectedCommittee, value))
            {
                if (value != null)
                {
                    FormDesignation = value.Designation;
                    FormApprenticeShip = value.ApprenticeShip;
                    FormTestDates = string.Join(", ", value.TestDates.Select(d => d.ToString("dd.MM.yyyy")));
                    IsEditing = true;
                }
                OnPropertyChanged(nameof(CanDelete));

                // Refresh available examinees when selection changes
                LoadAvailableExaminees();
                SelectedAssignExaminee = value?.Examinee;
                OnPropertyChanged(nameof(SelectedAssignExaminee));
            }
        }
    }

    // --- Form Fields ---
    private string _formDesignation = "";
    /// <summary>Form field for the committee designation.</summary>
    public string FormDesignation
    {
        get => _formDesignation;
        set => SetProperty(ref _formDesignation, value);
    }

    private string _formApprenticeShip = "";
    /// <summary>Form field for the apprenticeship type.</summary>
    public string FormApprenticeShip
    {
        get => _formApprenticeShip;
        set => SetProperty(ref _formApprenticeShip, value);
    }

    private string _formTestDates = "";
    /// <summary>Form field for comma-separated test dates.</summary>
    public string FormTestDates
    {
        get => _formTestDates;
        set => SetProperty(ref _formTestDates, value);
    }

    private bool _isEditing;
    /// <summary>Whether the form is in edit mode (vs. create mode).</summary>
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    /// <summary>Whether a committee is selected and can be deleted.</summary>
    public bool CanDelete => SelectedCommittee != null;

    // --- Examinee Assignment ---
    /// <summary>Examinees available for assignment.</summary>
    public ObservableCollection<Examinee> AvailableExaminees { get; } = new();

    private Examinee? _selectedAssignExaminee;
    /// <summary>The examinee selected for assignment to the current committee.</summary>
    public Examinee? SelectedAssignExaminee
    {
        get => _selectedAssignExaminee;
        set => SetProperty(ref _selectedAssignExaminee, value);
    }

    // --- Commands ---
    /// <summary>Command to save the current form (create or update).</summary>
    public ICommand SaveCommand { get; }
    /// <summary>Command to delete the selected committee.</summary>
    public ICommand DeleteCommand { get; }
    /// <summary>Command to start creating a new committee.</summary>
    public ICommand NewCommand { get; }
    /// <summary>Command to cancel the current form edit.</summary>
    public ICommand CancelCommand { get; }
    /// <summary>Command to assign the selected examinee to the selected committee.</summary>
    public ICommand AssignExamineeCommand { get; }
    /// <summary>Command to remove the assigned examinee from the selected committee.</summary>
    public ICommand RemoveExamineeCommand { get; }

    /// <summary>Initializes the committee list view model with the given service.</summary>
    /// <param name="service">The backend service.</param>
    public CommitteeListViewModel(Service service)
    {
        _service = service;
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete, () => CanDelete);
        NewCommand = new RelayCommand(NewCommittee);
        CancelCommand = new RelayCommand(Cancel);
        AssignExamineeCommand = new RelayCommand(AssignExaminee);
        RemoveExamineeCommand = new RelayCommand(RemoveExaminee);
        LoadData();
    }

    /// <summary>Reloads all committees and available examinees from the service.</summary>
    public void LoadData()
    {
        Committees.Clear();
        foreach (var c in _service.GetAllCommittees())
            Committees.Add(c);
        LoadAvailableExaminees();
    }

    private void LoadAvailableExaminees()
    {
        AvailableExaminees.Clear();
        foreach (var e in _service.GetAllExaminees())
            AvailableExaminees.Add(e);
    }

    private List<DateTime> ParseTestDates(string input)
    {
        var dates = new List<DateTime>();
        if (string.IsNullOrWhiteSpace(input)) return dates;
        foreach (var part in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (DateTime.TryParse(part.Trim(), out var dt))
                dates.Add(dt);
        }
        return dates;
    }

    private void Save()
    {
        var dates = ParseTestDates(FormTestDates);

        if (string.IsNullOrWhiteSpace(FormDesignation))
        {
            MessageBox.Show("Bezeichnung darf nicht leer sein.", "Validierung", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsEditing && SelectedCommittee != null)
        {
            _service.UpdateCommittee(SelectedCommittee,
                designation: FormDesignation,
                apprenticeShip: FormApprenticeShip,
                testDates: dates);
        }
        else
        {
            _service.AddCommittee(FormDesignation, FormApprenticeShip, dates);
        }

        ClearForm();
        LoadData();
    }

    private void Delete()
    {
        if (SelectedCommittee == null) return;

        var result = MessageBox.Show(
            $"Prüfungsausschuss \"{SelectedCommittee.Designation}\" wirklich löschen?",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _service.RemoveCommittee(SelectedCommittee);
            ClearForm();
            LoadData();
        }
    }

    private void NewCommittee()
    {
        ClearForm();
    }

    private void Cancel()
    {
        ClearForm();
    }

    private void AssignExaminee()
    {
        if (SelectedCommittee == null || SelectedAssignExaminee == null) return;
        _service.AssignExamineeToCommittee(SelectedCommittee, SelectedAssignExaminee);
        LoadData();
    }

    private void RemoveExaminee()
    {
        if (SelectedCommittee == null) return;
        _service.RemoveExamineeFromCommittee(SelectedCommittee);
        LoadData();
    }

    private void ClearForm()
    {
        FormDesignation = "";
        FormApprenticeShip = "";
        FormTestDates = "";
        IsEditing = false;
        SelectedCommittee = null;
    }
}
