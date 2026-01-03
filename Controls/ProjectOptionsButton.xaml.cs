using System.Windows;
using System.Windows.Input;

namespace UnrealProjectHub.Controls;

public partial class ProjectOptionsButton
{
    public ICommand OpenIdeCommand
    {
        get => (ICommand)GetValue(OpenIdeCommandProperty);
        set => SetValue(OpenIdeCommandProperty, value);
    }
    public ICommand OpenInExplorerCommand
    {
        get => (ICommand)GetValue(OpenInExplorerProperty);
        set => SetValue(OpenInExplorerProperty, value);
    }
    public ICommand RebuildAndLaunchCommand
    {
        get => (ICommand)GetValue(RebuildAndLaunchProperty);
        set => SetValue(RebuildAndLaunchProperty, value);
    }
    public ICommand RemoveProjectCommand
    {
        get => (ICommand)GetValue(RemoveProjectProperty);
        set => SetValue(RemoveProjectProperty, value);
    }

    public static readonly DependencyProperty OpenIdeCommandProperty = DependencyProperty.Register(
        nameof(OpenIdeCommand),
        typeof(ICommand),
        typeof(ProjectOptionsButton),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty OpenInExplorerProperty = DependencyProperty.Register(
        nameof(OpenInExplorerCommand),
        typeof(ICommand),
        typeof(ProjectOptionsButton),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty RebuildAndLaunchProperty = DependencyProperty.Register(
        nameof(RebuildAndLaunchCommand),
        typeof(ICommand),
        typeof(ProjectOptionsButton),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty RemoveProjectProperty = DependencyProperty.Register(
        nameof(RemoveProjectCommand),
        typeof(ICommand),
        typeof(ProjectOptionsButton),
        new PropertyMetadata(null)
    );
    
    public ProjectOptionsButton()
    {
        InitializeComponent();
    }

    private void OpenPopup(object _, RoutedEventArgs __)
    {
        Popup.PlacementTarget = OptionsButton;
        Popup.IsOpen = true;
    }
}