using System.Windows;

namespace EvalProUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // WICHTIG: public PROPERTY, kein Feld!
        // Gefixt: Rückgabe war rekursiv und hat zu StackOverflow geführt.
        public string GreetingName
        {
            get { return $"Guten Morgen, Benutzer"; }
        }
    }
}