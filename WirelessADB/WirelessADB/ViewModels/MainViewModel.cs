using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WirelessADB.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public Window Window { get; set; }
    [ObservableProperty]
    private string platformToolsPath;

    [ObservableProperty]
    private string ip;

    [ObservableProperty]
    private string port;

    [ObservableProperty]
    private string resultText;

    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    [ObservableProperty]
    private bool isConnected;
    public bool IsNotConnected { get => !IsConnected; }

    public ObservableCollection<string> CommandsExecuted { get; } = new ObservableCollection<string>();

    public ICommand BrowseCommand { get; }
    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }

    public MainViewModel()
    {
        BrowseCommand = new AsyncRelayCommand(BrowsePlatformTools);
        ConnectCommand = new AsyncRelayCommand(Connect);
        DisconnectCommand = new AsyncRelayCommand(Disconnect);

        LoadSettings();
    }

    private async Task BrowsePlatformTools()
    {
        var dialog = new OpenFolderDialog();
        var result = await dialog.ShowAsync(Window);

        if (result != null)
        {
            PlatformToolsPath = result;
            SaveSettings();
        }
    }

    private async Task Connect()
    {
        SaveSettings();

        var adbPath = Path.Combine(PlatformToolsPath, "adb");
        ResultText = "Loading...";
        string command = $"{adbPath} connect {Ip}:{Port}";
        CommandsExecuted.Add("COMMAND: " + command);
        var result = await ExecuteAdbCommand(command);
        CommandsExecuted.Add("RESPONSE: " + result);

        ResultText = result;

        if (result.Contains("connected"))
        {
            IsConnected = true;
        }
        CheckConnectionStatus();
    }

    private async Task Disconnect()
    {
        var adbPath = Path.Combine(PlatformToolsPath, "adb");
        ResultText = "Loading...";
        string command = $"{adbPath} disconnect {Ip}:{Port}";
        var result = await ExecuteAdbCommand(command);
        CommandsExecuted.Add("COMMAND: " + command);
        CommandsExecuted.Add("RESPONSE: " + result);

        ResultText = result;

        IsConnected = false;
        CheckConnectionStatus();
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

    private async void CheckConnectionStatus()
    {
        if (string.IsNullOrEmpty(PlatformToolsPath) || string.IsNullOrEmpty(Ip)) return;
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
}

