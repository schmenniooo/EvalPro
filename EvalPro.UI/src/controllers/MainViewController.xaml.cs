using System.Windows;
using EvalProUI.model;
using Service = EvalProService.impl.service.EvalProService;

namespace EvalProUI
{
    /// <summary>Main application window hosting navigation and content views.</summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        /// <summary>Initializes the main window with the given service instance.</summary>
        /// <param name="service">The backend service.</param>
        public MainWindow(Service service)
        {
            InitializeComponent();
            _viewModel = new MainViewModel(service);
            DataContext = _viewModel;
        }
    }
}