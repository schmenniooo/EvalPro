using System.Windows;

namespace WPFApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // WICHTIG: public PROPERTY, kein Feld!
        public string GreetingName
        {
            get { return $"Guten Morgen, {GreetingName}"; }
        }
    }
}