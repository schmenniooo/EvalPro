namespace EvalProUI;

using System;
using System.Windows;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var app = new Application();
        var main = new MainWindow();
        app.Run(main);
    }
}