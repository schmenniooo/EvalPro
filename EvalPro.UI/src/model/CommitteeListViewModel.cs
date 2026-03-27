using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EvalProService.impl.model.entities;
using EvalProService.impl.service;

namespace EvalProUI.model;

/// <summary>
/// ViewModel for the Audit Committee list view with CRUD operations.
/// </summary>
public class CommitteeListViewModel : BaseViewModel
{
    private readonly EvalProService _service;

    public ObservableCollection<AuditCommittee> Committees { get; } = new();

    private AuditCommittee? _selectedCommittee;
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
    public string FormDesignation
    {
        get => _formDesignation;
        set => SetProperty(ref _formDesignation, value);
    }

    private string _formApprenticeShip = "";
    public string FormApprenticeShip
    {
        get => _formApprenticeShip;
        set => SetProperty(ref _formApprenticeShip, value);
    }

    private string _formTestDates = "";
    public string FormTestDates
    {
        get => _formTestDates;
        set => SetProperty(ref _formTestDates, value);
    }

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public bool CanDelete => SelectedCommittee != null;

    // --- Examinee Assignment ---
    public ObservableCollection<Examinee> AvailableExaminees { get; } = new();

    private Examinee? _selectedAssignExaminee;
    public Examinee? SelectedAssignExaminee
    {
        get => _selectedAssignExaminee;
        set => SetProperty(ref _selectedAssignExaminee, value);
    }

    // --- Commands ---
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AssignExamineeCommand { get; }
    public ICommand RemoveExamineeCommand { get; }

    public CommitteeListViewModel(EvalProService service)
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
