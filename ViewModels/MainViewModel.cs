using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
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

    private static readonly string CacheFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UnrealEngineHub",
        "projects.json"
    );

    public MainViewModel()
    {
        InitCommands(new UnrealProjectService(Log));
        LoadProjects();
        SortProjects();
    }

    private async Task SafeRebuildAndLaunchAsync(UnrealProjectService service, ProjectEntry project)
    {
        ClearLogs();

        try
        {
            await RebuildAndLaunchAsync(service, project);
        }
        catch (Exception caughtException)
        {
            Log($"Error: {caughtException.Message}");
        }
    }

    private void AddProject()
    {
        if (!TryAddProject())
        {
            return;
        }

        SortProjects();
        SaveProjects();
    }

    private void SaveProjects()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CacheFile)!);

        File.WriteAllText(CacheFile, JsonSerializer.Serialize(Projects, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private void LoadProjects()
    {
        if (!File.Exists(CacheFile))
        {
            return;
        }

        List<ProjectEntry>? projects = JsonSerializer.Deserialize<List<ProjectEntry>>(File.ReadAllText(CacheFile));

        if (projects == null)
        {
            return;
        }

        foreach (ProjectEntry project in projects.OrderByDescending(p => p.LastOpened))
        {
            Projects.Add(project);
        }
    }

    private void SortProjects()
    {
        List<ProjectEntry> sortedProjects = Projects.OrderByDescending(p => p.LastOpened).ToList();

        Projects.Clear();
        
        foreach (ProjectEntry project in sortedProjects)
        {
            Projects.Add(project);
        }
    }

    private bool TryAddProject()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Unreal Project (*.uproject)|*.uproject"
        };

        if (dialog.ShowDialog() != true)
        {
            return false;
        }

        string uproject = dialog.FileName;

        if (Projects.Any(p => string.Equals(p.UProjectPath, uproject, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        string directory = Path.GetDirectoryName(uproject)!;
        string sln = Directory.GetFiles(directory, "*.sln").FirstOrDefault() ?? "";

        Projects.Add(new ProjectEntry
        {
            Name = Path.GetFileNameWithoutExtension(uproject),
            UProjectPath = uproject,
            SolutionPath = sln,
            Directory = directory,
            LastOpened = DateTime.Now
        });
        
        return true;
    }

    private void RemoveProject(ProjectEntry project)
    {
        Projects.Remove(project);
    }

    private void ClearLogs()
    {
        Logs.Clear();
    }

    private void Log(string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        });
    }

    private async Task RebuildAndLaunchAsync(UnrealProjectService service, ProjectEntry project)
    {
        await service.RebuildAndLaunchAsync(project.UProjectPath);
    }

    private void OpenInExplorer(UnrealProjectService service, ProjectEntry project)
    {
        service.OpenInExplorer(project.Directory);
    }

    private void OpenIde(UnrealProjectService service, ProjectEntry project)
    {
        service.LaunchIde(project.SolutionPath);
    }

    private void OpenUnrealEngine(UnrealProjectService service, ProjectEntry project)
    {
        service.LaunchUnrealEngine(project.UProjectPath);
    }

    private void InitCommands(UnrealProjectService service)
    {
        RebuildAndLaunchCommand = new RelayCommand(project => _ = SafeRebuildAndLaunchAsync(service, project));
        OpenUnrealEngineCommand = new RelayCommand(project => OpenUnrealEngine(service, project));
        OpenInExplorerCommand = new RelayCommand(project => OpenInExplorer(service, project));
        OpenIdeCommand = new RelayCommand(project => OpenIde(service, project));
        RemoveProjectCommand = new RelayCommand(project =>
        {
            RemoveProject(project);
            SaveProjects();
        });
        AddProjectCommand = new SimpleCommand(AddProject);
    }
}
