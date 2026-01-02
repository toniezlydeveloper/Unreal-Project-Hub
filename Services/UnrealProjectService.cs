using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace UnrealProjectHub.Services;

public class UnrealProjectService(Action<string> log)
{
    private static readonly string[] DirectoriesToClean = [".vs", "Binaries", "Intermediate", "DerivedDataCache"];

    private const int UnrealEngineKillCheckIntervalInMs = 250;
    private const int UnrealEngineKillTimeoutInMs = 8_000;

    public void LaunchUnrealEngine(string uprojectPath)
    {
        Log("Starting Unreal Engine");
        
        Process.Start(new ProcessStartInfo
        {
            FileName = uprojectPath,
            UseShellExecute = true
        });
    }

    public void LaunchIde(string slnPath)
    {
        if (string.IsNullOrEmpty(slnPath))
        {
            return;
        }

        Log("Launching IDE");
        
        Process.Start(new ProcessStartInfo
        {
            FileName = slnPath,
            UseShellExecute = true
        });
    }

    public void OpenInExplorer(string directory)
    {
        Log("Opening in Explorer");
        
        Process.Start(new ProcessStartInfo
        {
            FileName = directory,
            UseShellExecute = true
        });
    }
    
    public async Task RebuildAndLaunchAsync(string uprojectPath)
    {
        Log("Closing Unreal Editor...");
        KillUnrealEngine(GetProcesses());
        
        if (!WaitForUnrealEngineKill())
        {
            throw new Exception("Unreal Editor failed to close.");
        }
        
        Log("Cleaning project directories...");
        Clean(uprojectPath);

        Log("Generating Visual Studio files...");
        await GenerateIdeFilesAsync(uprojectPath);

        Log("Launching Unreal Editor...");
        LaunchUnrealEngine(uprojectPath);

        Log("Done.");
    }

    private Process[] GetProcesses() => Process.GetProcessesByName("UnrealEditor");

    private bool WaitForUnrealEngineKill()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        while (GetProcesses().Any())
        {
            if (HasTimedOut(stopwatch))
            {
                return false;
            }

            Thread.Sleep(UnrealEngineKillCheckIntervalInMs);
        }

        return true;
    }

    private async Task GenerateIdeFilesAsync(string uproject)
    {
        string engine = ResolveEngine(uproject);
        string unrealBuildTool = Path.Combine(engine, "Engine", "Binaries", "DotNET", "UnrealBuildTool", "UnrealBuildTool.dll");
        Log("Generating IDE files...");
        await RunProcessAsync("dotnet", $"\"{unrealBuildTool}\" -projectfiles -project=\"{uproject}\" -game -rocket -progress");
    }

    private void Clean(string uproject)
    {
        string root = Path.GetDirectoryName(uproject)!;

        foreach (string directory in DirectoriesToClean)
        {
            string path = Path.Combine(root, directory);

            if (!Directory.Exists(path))
            {
                continue;
            }
            
            Log($"Deleting {directory}");
            Directory.Delete(path, true);
        }
    }
    
    private bool HasTimedOut(Stopwatch stopwatch) => stopwatch.ElapsedMilliseconds > UnrealEngineKillTimeoutInMs;
    
    private void KillUnrealEngine(Process[] processes)
    {
        if (processes.Length == 0)
        {
            return;
        }

        foreach (Process process in processes)
        {
            try
            {
                log($"Closing UnrealEditor (PID {process.Id})");
                process.Kill();
            }
            catch (Exception caughtException)
            {
                log($"Failed to kill process {process.Id}: {caughtException.Message}");
            }
        }
    }
    
    private async Task RunProcessAsync(string exe, string args)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data != null)
            {
                log(eventArgs.Data);
            }
        };

        process.ErrorDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data != null)
            {
                log(eventArgs.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();
    }
    
    private string ResolveEngine(string uproject)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(uproject));

        if (!doc.RootElement.TryGetProperty("EngineAssociation", out JsonElement associationProperty))
        {
            throw new Exception("EngineAssociation property is missing.");
        }

        string engineId = associationProperty.GetString()!;

        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Epic Games\Unreal Engine\Builds"))
        {
            string? path = key?.GetValue(engineId)?.ToString();
            
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Epic Games\Unreal Engine\Builds"))
        {
            string? path = key?.GetValue(engineId)?.ToString();
            
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        using (RegistryKey? key = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\EpicGames\Unreal Engine\{engineId}"))
        {
            string? path = key?.GetValue("InstalledDirectory")?.ToString();
            
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        throw new Exception($"Unable to resolve Unreal Engine {engineId}");
    }

    private void Log(string msg) => log(msg);
}
