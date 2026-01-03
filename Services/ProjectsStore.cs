using System.IO;
using System.Text.Json;
using UnrealProjectHub.Models;

namespace UnrealProjectHub.Services;

public class ProjectsStore(Action<IReadOnlyCollection<ProjectEntry>> refresh)
{
    private readonly List<ProjectEntry> _projects = [];
    
    private static readonly string CacheFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UnrealEngineHub",
        "projects.json"
    );
    
    public void AddProject(string uproject)
    {
        if (!TryAddProject(uproject))
        {
            return;
        }

        SortProjects();
        SaveProjects();
        Refresh();
    }

    public void Init()
    {
        LoadProjects();
        Refresh();
    }

    public void RemoveProject(ProjectEntry project)
    {
        Remove(project);
        SaveProjects();
        Refresh();
    }

    private void SaveProjects()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CacheFile)!);

        File.WriteAllText(CacheFile, JsonSerializer.Serialize(_projects, new JsonSerializerOptions
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

        List<ProjectEntry>? loadedProjects = JsonSerializer.Deserialize<List<ProjectEntry>>(File.ReadAllText(CacheFile));

        if (loadedProjects == null)
        {
            return;
        }

        foreach (ProjectEntry project in loadedProjects.OrderByDescending(p => p.LastOpened))
        {
            _projects.Add(project);
        }
    }

    private void SortProjects()
    {
        List<ProjectEntry> sortedProjects = _projects.OrderByDescending(p => p.LastOpened).ToList();
        
        _projects.Clear();
        
        foreach (ProjectEntry project in sortedProjects)
        {
            _projects.Add(project);
        }
    }

    private bool TryAddProject(string uproject)
    {
        if (_projects.Any(p => string.Equals(p.UProjectPath, uproject, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        string directory = Path.GetDirectoryName(uproject)!;
        string sln = Directory.GetFiles(directory, "*.sln").FirstOrDefault() ?? "";

        _projects.Add(new ProjectEntry
        {
            Name = Path.GetFileNameWithoutExtension(uproject),
            UProjectPath = uproject,
            SolutionPath = sln,
            Directory = directory,
            LastOpened = DateTime.Now
        });
        
        return true;
    }

    private void Remove(ProjectEntry project)
    {
        _projects.Remove(project);
    }

    private void Refresh()
    {
        refresh.Invoke(_projects);
    }
}