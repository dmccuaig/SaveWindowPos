using System.ComponentModel;
using System.Windows;
using SaveWindowPos;
using SaveWindowPosDemo.Properties;

namespace SaveWindowPosDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        this.RestoreWindowPosition(Settings.Default.WindowPlacement);
        this.EnsureIsVisible();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Settings.Default.WindowPlacement = this.GetWindowPositionString();
        Settings.Default.Save();
    }
}