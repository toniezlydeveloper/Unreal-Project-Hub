using System.Windows;
using System.Windows.Input;

namespace UnrealProjectHub.Controls;

public partial class ProjectsHeader
{
    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
    }
    
    public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register(
        nameof(ClickCommand),
        typeof(ICommand),
        typeof(ProjectsHeader)
    );
    
    public ProjectsHeader()
    {
        InitializeComponent();
    }
}