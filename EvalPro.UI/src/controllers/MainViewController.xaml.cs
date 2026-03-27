using System.Windows;
using EvalProUI.model;
using EvalProService.impl.service;

namespace EvalProUI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(EvalProService service)
        {
            InitializeComponent();
            _viewModel = new MainViewModel(service);
            DataContext = _viewModel;
        }
    }
}