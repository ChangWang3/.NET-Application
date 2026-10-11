using System;
using System.Windows;
using WpfApp;

internal static class Program
{
    // Preview only: open Eugene's window without changing the team's startup code.
    [STAThread]
    private static void Main()
    {
        Application application = new Application();
        application.Run(new CsvWorkspace());
    }
}
