using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EvalProService.impl.model.entities;
using EvalProService.impl.model.ratings;
using Service = EvalProService.impl.service.EvalProService;

namespace EvalProUI.model;

/// <summary>
/// ViewModel for the examinee detail/rating view.
/// Shows examinee info and allows assigning ratings.
/// </summary>
public class ExamineeDetailViewModel : BaseViewModel
{
    private readonly Service _service;
    private readonly Examinee _examinee;
    private readonly Action _navigateBack;

    /// <summary>Name of the examinee.</summary>
    public string ExamineeName => _examinee.Name;
    /// <summary>Company of the examinee.</summary>
    public string ExamineeCompany => _examinee.Company;
    /// <summary>Contact person for the examinee.</summary>
    public string ExamineeContactPerson => _examinee.ContactPerson;
    /// <summary>Project title of the examinee.</summary>
    public string ExamineeProjectTitle => _examinee.ProjectTitle;

    /// <summary>Display text for the assigned committee, or a fallback message.</summary>
    public string AssignedCommittee
    {
        get
        {
            var c = _service.GetCommitteeForExaminee(_examinee);
            return c != null ? $"{c.Designation} ({c.ApprenticeShip})" : "Nicht zugewiesen";
        }
    }

    // --- Rating Status ---
    /// <summary>Status text for the project documentation rating.</summary>
    public string DocStatus => _examinee.ProjectDocumentation != null ? "✓ Bewertet" : "○ Ausstehend";
    /// <summary>Status text for the project presentation rating.</summary>
    public string PresStatus => _examinee.ProjectPresentation != null ? "✓ Bewertet" : "○ Ausstehend";
    /// <summary>Status text for the tech conversation rating.</summary>
    public string TechStatus => _examinee.TechConversation != null ? "✓ Bewertet" : "○ Ausstehend";
    /// <summary>Status text for the supplementary examination rating.</summary>
    public string SuppStatus => _examinee.SupplementaryExamination != null ? "✓ Bewertet" : "○ Ausstehend";

    // --- Project Documentation Form ---
    private string _docCriteria = "";
    /// <summary>Form field for project documentation criteria (points and comments).</summary>
    public string DocCriteria { get => _docCriteria; set => SetProperty(ref _docCriteria, value); }
    private string _docComment = "";
    /// <summary>Form field for the project documentation final comment.</summary>
    public string DocComment { get => _docComment; set => SetProperty(ref _docComment, value); }

    // --- Project Presentation Form ---
    private string _presCriteria = "";
    /// <summary>Form field for project presentation criteria (points and comments).</summary>
    public string PresCriteria { get => _presCriteria; set => SetProperty(ref _presCriteria, value); }
    private string _presComment = "";
    /// <summary>Form field for the project presentation final comment.</summary>
    public string PresComment { get => _presComment; set => SetProperty(ref _presComment, value); }

    // --- Tech Conversation Form ---
    private string _techCriteria = "";
    /// <summary>Form field for tech conversation criteria (points and comments).</summary>
    public string TechCriteria { get => _techCriteria; set => SetProperty(ref _techCriteria, value); }
    private string _techComment = "";
    /// <summary>Form field for the tech conversation final comment.</summary>
    public string TechComment { get => _techComment; set => SetProperty(ref _techComment, value); }

    // --- Supplementary Examination Form ---
    private string _suppTestArea = "";
    /// <summary>Form field for the supplementary examination test area.</summary>
    public string SuppTestArea { get => _suppTestArea; set => SetProperty(ref _suppTestArea, value); }
    private string _suppPoints = "0";
    /// <summary>Form field for the supplementary examination points.</summary>
    public string SuppPoints { get => _suppPoints; set => SetProperty(ref _suppPoints, value); }
    private string _suppQuestions = "";
    /// <summary>Form field for the supplementary examination questions (one per line).</summary>
    public string SuppQuestions { get => _suppQuestions; set => SetProperty(ref _suppQuestions, value); }

    // --- Commands ---
    /// <summary>Command to save the project documentation rating.</summary>
    public ICommand SaveDocCommand { get; }
    /// <summary>Command to save the project presentation rating.</summary>
    public ICommand SavePresCommand { get; }
    /// <summary>Command to save the tech conversation rating.</summary>
    public ICommand SaveTechCommand { get; }
    /// <summary>Command to save the supplementary examination rating.</summary>
    public ICommand SaveSuppCommand { get; }
    /// <summary>Command to navigate back to the examinee list.</summary>
    public ICommand BackCommand { get; }

    /// <summary>Initializes the examinee detail view model.</summary>
    /// <param name="service">The backend service.</param>
    /// <param name="examinee">The examinee to display.</param>
    /// <param name="navigateBack">Callback to navigate back to the list view.</param>
    public ExamineeDetailViewModel(Service service, Examinee examinee, Action navigateBack)
    {
        _service = service;
        _examinee = examinee;
        _navigateBack = navigateBack;

        SaveDocCommand = new RelayCommand(SaveDoc);
        SavePresCommand = new RelayCommand(SavePres);
        SaveTechCommand = new RelayCommand(SaveTech);
        SaveSuppCommand = new RelayCommand(SaveSupp);
        BackCommand = new RelayCommand(() => _navigateBack());

        LoadExistingRatings();
    }

    private void LoadExistingRatings()
    {
        if (_examinee.ProjectDocumentation is { } doc)
        {
            DocCriteria = FormatCriteria(doc.PointsPerCriteria, doc.CommentsPerCriteria);
            DocComment = doc.FinalComment;
        }

        if (_examinee.ProjectPresentation is { } pres)
        {
            PresCriteria = FormatCriteria(pres.PointsPerCriteria, pres.CommentsPerCriteria);
            PresComment = pres.FinalComment;
        }

        if (_examinee.TechConversation is { } tech)
        {
            TechCriteria = FormatCriteria(tech.PointsPerCriteria, tech.CommentsPerCriteria);
            TechComment = tech.FinalComment;
        }

        if (_examinee.SupplementaryExamination is { } supp)
        {
            SuppTestArea = supp.ChosenTestArea;
            SuppPoints = supp.Points.ToString();
            SuppQuestions = string.Join("\n", supp.Questions);
        }
    }

    private string FormatCriteria(Dictionary<string, int> points, Dictionary<string, string> comments)
    {
        var lines = new List<string>();
        foreach (var kvp in points)
        {
            var comment = comments.GetValueOrDefault(kvp.Key, "");
            lines.Add($"{kvp.Key}: {kvp.Value} Punkte" + (string.IsNullOrEmpty(comment) ? "" : $" - {comment}"));
        }
        return string.Join("\n", lines);
    }

    private (Dictionary<string, int> points, Dictionary<string, string> comments) ParseCriteria(string input)
    {
        var points = new Dictionary<string, int>();
        var comments = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(input)) return (points, comments);

        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var colonIdx = line.IndexOf(':');
            if (colonIdx < 0) continue;

            var key = line[..colonIdx].Trim();
            var rest = line[(colonIdx + 1)..].Trim();

            // Try to parse "X Punkte - comment" or just "X"
            var dashIdx = rest.IndexOf('-');
            string pointsPart, commentPart = "";

            if (dashIdx >= 0)
            {
                pointsPart = rest[..dashIdx].Trim();
                commentPart = rest[(dashIdx + 1)..].Trim();
            }
            else
            {
                pointsPart = rest;
            }

            // Remove "Punkte" suffix if present
            pointsPart = pointsPart.Replace("Punkte", "").Trim();

            if (int.TryParse(pointsPart, out var p))
            {
                points[key] = p;
                comments[key] = commentPart;
            }
        }

        return (points, comments);
    }

    private void SaveDoc()
    {
        var (points, comments) = ParseCriteria(DocCriteria);
        var doc = new ProjectDocumentation(DocComment, points, comments);
        _service.AssignProjectDocumentation(_examinee, doc);
        RefreshStatuses();
        MessageBox.Show("Dokumentationsbewertung gespeichert.", "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SavePres()
    {
        var (points, comments) = ParseCriteria(PresCriteria);
        var pres = new ProjectPresentation(PresComment, points, comments);
        _service.AssignProjectPresentation(_examinee, pres);
        RefreshStatuses();
        MessageBox.Show("Präsentationsbewertung gespeichert.", "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SaveTech()
    {
        var (points, comments) = ParseCriteria(TechCriteria);
        var tech = new TechConversation(TechComment, points, comments);
        _service.AssignTechConversation(_examinee, tech);
        RefreshStatuses();
        MessageBox.Show("Fachgesprächsbewertung gespeichert.", "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SaveSupp()
    {
        if (!int.TryParse(SuppPoints, out var pts))
        {
            MessageBox.Show("Punkte müssen eine Zahl sein.", "Validierung", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var questions = SuppQuestions.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        var exam = new SupplementaryExamination(SuppTestArea, pts, questions);
        _service.AssignSupplementaryExamination(_examinee, exam);
        RefreshStatuses();
        MessageBox.Show("Ergänzungsprüfung gespeichert.", "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void RefreshStatuses()
    {
        OnPropertyChanged(nameof(DocStatus));
        OnPropertyChanged(nameof(PresStatus));
        OnPropertyChanged(nameof(TechStatus));
        OnPropertyChanged(nameof(SuppStatus));
    }
}
