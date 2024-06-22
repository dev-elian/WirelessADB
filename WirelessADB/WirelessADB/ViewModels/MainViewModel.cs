using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WirelessADB.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public Window Window { get; set; }

    [ObservableProperty]
    private string platformToolsPath = "";

    [ObservableProperty]
    private string ip = "";

    [ObservableProperty]
    private string port = "";

    [ObservableProperty]
    private string pairCode = "";

    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    [ObservableProperty]
    private bool isConnected;
    public bool IsNotConnected { get => !IsConnected; }

    public ObservableCollection<string> CommandsHistory { get; } = [];

    public MainViewModel()
    {
        LoadSettings();
    }

    [RelayCommand]
    private async Task Browse()
    {
        var dialog = new OpenFolderDialog();
        var result = await dialog.ShowAsync(Window);

        if (result != null)
        {
            PlatformToolsPath = result;
            SaveSettings();
        }
    }

    [RelayCommand]
    private async Task Connect()
    {
        SaveSettings();

        var adbPath = Path.Combine(PlatformToolsPath, "adb");
        string command = $"{adbPath} connect {Ip}:{Port}";
        CommandsHistory.Add("COMMAND: " + command);
        var result = await ExecuteAdbCommand(command);
        CommandsHistory.Add("RESPONSE: " + result);

        if (result.Contains("connected"))
        {
            IsConnected = true;
        }
        CheckConnectionStatus();
    }

    [RelayCommand]
    private async Task Disconnect()
    {
        var adbPath = Path.Combine(PlatformToolsPath, "adb");
        string command = $"{adbPath} disconnect {Ip}:{Port}";
        var result = await ExecuteAdbCommand(command);
        CommandsHistory.Add("COMMAND: " + command);
        CommandsHistory.Add("RESPONSE: " + result);

        IsConnected = false;
        CheckConnectionStatus();
    }

    [RelayCommand]
    private async Task PairDevice()
    {
        if (string.IsNullOrEmpty(PairCode.Trim()))
        {
            CommandsHistory.Add("Inserte un código de emparejamiento");
            return;
        }
        var adbPath = Path.Combine(PlatformToolsPath, "adb");
        CommandsHistory.Add("Code: " + PairCode);
        var result2 = await ExecuteAdbCommand($"{adbPath} pair {Ip}:{Port} {PairCode}");
        CommandsHistory.Add("RESPONSE: " + result2);
    }

    private void LoadSettings()
    {
        if (File.Exists("settings.txt"))
        {
            var lines = File.ReadAllLines("settings.txt");
            PlatformToolsPath = lines[0];
            Ip = lines[1];
        }

        CheckConnectionStatus();
    }
    public void SaveSettings()
    {
        File.WriteAllLines("settings.txt", new[] { PlatformToolsPath, Ip });
    }

    private async void CheckConnectionStatus()
    {
        if (string.IsNullOrEmpty(PlatformToolsPath.Trim()) || string.IsNullOrEmpty(Ip.Trim()))
        {
            IsConnected = false;
            CommandsHistory.Add("Verifique la IP y la dirección del SDK");
            return;
        }
        var adbPath = Path.Combine(PlatformToolsPath, "adb");
        var result = await ExecuteAdbCommand($"{adbPath} devices");

        if (result.Contains(Ip))
        {
            Port = result.Split(":")[1].Substring(0,5);
            IsConnected = true;
        }
        else
        {
            IsConnected = false;
        }
    }
    private async Task<string> ExecuteAdbCommand(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string result = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();

        return result;
    }
}

