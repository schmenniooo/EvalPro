namespace EvalProUI;

using System;
using System.Windows;
using EvalProService.impl.model.events;
using EvalProService.impl.service;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        using var service = new EvalProService();

        // Surface auto-save errors to the user
        service.OnSaveError += (_, args) =>
        {
            var severity = args.IsCritical ? "KRITISCH" : "Warnung";
            MessageBox.Show(
                $"Fehler beim Speichern: {args.Exception.Message}",
                $"Speicherfehler ({severity})",
                MessageBoxButton.OK,
                args.IsCritical ? MessageBoxImage.Error : MessageBoxImage.Warning);
        };

        var app = new Application();
        var main = new MainWindow(service);
        app.Run(main);
    }
}