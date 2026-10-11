using System;
using System.Windows;
using WpfApp;

internal static class Program
{
    // Open Eugene's window without changing the team's main menu or startup code.
    [STAThread]
    private static void Main()
    {
        Application application = new Application();
        application.Run(new ProbabilityWindow());
    }
}
