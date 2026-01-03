using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using UnrealProjectHub.Commands;
using UnrealProjectHub.Models;
using UnrealProjectHub.Services;

namespace UnrealProjectHub.ViewModels;

public class MainViewModel
{
    public ObservableCollection<ProjectEntry> Projects { get; } = [];
    public ObservableCollection<string> Logs { get; } = [];

    public SimpleCommand AddProjectCommand { get; private set; }
    public RelayCommand RemoveProjectCommand { get; private set; }
    public RelayCommand OpenUnrealEngineCommand { get; private set; }
    public RelayCommand OpenIdeCommand { get; private set; }
    public RelayCommand OpenInExplorerCommand { get; private set; }
    public RelayCommand RebuildAndLaunchCommand { get; private set; }

    public MainViewModel()
    {
        UnrealToolchain toolchain = new(Log, ClearLogs);
        ProjectsStore store = new(Refresh);
        InitCommands(toolchain, store);
        Init(store);
    }

    private void InitCommands(UnrealToolchain toolchain, ProjectsStore store)
    {
        RebuildAndLaunchCommand = new RelayCommand(project =>
        {
            try
            {
                _ = RebuildAndLaunchAsync(toolchain, project);
            }
            catch (Exception caughtException)
            {
                Log($"Error: {caughtException.Message}");
            }
        });
        OpenUnrealEngineCommand = new RelayCommand(project => OpenUnrealEngine(toolchain, project));
        OpenInExplorerCommand = new RelayCommand(project => OpenInExplorer(toolchain, project));
        OpenIdeCommand = new RelayCommand(project => OpenIde(toolchain, project));
        
        RemoveProjectCommand = new RelayCommand(project => RemoveProject(store, project));
        AddProjectCommand = new SimpleCommand(() => AddProject(store));
    }

    private async Task RebuildAndLaunchAsync(UnrealToolchain toolchain, ProjectEntry project)
    {
        await toolchain.RebuildAndLaunchAsync(project.UProjectPath);
    }

    private void OpenInExplorer(UnrealToolchain toolchain, ProjectEntry project)
    {
        toolchain.OpenInExplorer(project.Directory);
    }

    private void OpenIde(UnrealToolchain toolchain, ProjectEntry project)
    {
        toolchain.LaunchIde(project.SolutionPath);
    }

    private void OpenUnrealEngine(UnrealToolchain toolchain, ProjectEntry project)
    {
        toolchain.LaunchUnrealEngine(project.UProjectPath);
    }

    private void AddProject(ProjectsStore store)
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Unreal Project (*.uproject)|*.uproject"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        store.AddProject(dialog.FileName);
    }

    private void RemoveProject(ProjectsStore store, ProjectEntry project)
    {
        store.RemoveProject(project);
    }

    private void Init(ProjectsStore store)
    {
        store.Init();
    }

    private void Log(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        });
    }

    private void ClearLogs()
    {
        Logs.Clear();
    }

    private void Refresh(IReadOnlyCollection<ProjectEntry> projects)
    {
        Projects.Clear();
        
        foreach (ProjectEntry project in projects)
        {
            Projects.Add(project);
        }
    }
}
