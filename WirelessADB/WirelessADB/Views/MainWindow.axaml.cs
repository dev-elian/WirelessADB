using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using WirelessADB.ViewModels;

namespace WirelessADB.Views;

public partial class MainWindow : Window
{
    MainViewModel vm;
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        vm = DataContext as MainViewModel;
        vm.Window = this;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {        
        vm.SaveSettings();
        base.OnClosing(e);
    }

    private void OpenUrl_Click(object sender, RoutedEventArgs e)
    {
        var url = "https://www.youtube.com/@GeekHack-sy4qv";
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }


    private void OpenUrl2_Click(object sender, RoutedEventArgs e)
    {
        var url = "https://github.com/dev-elian/WirelessADB";
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }


    //Los comandos no deberian estar aqui pero al compilar como aplicacion self-contained realizando binding a los comandos no funcionan los botones
    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        vm.BrowseCommand.Execute(null);
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        vm.ConnectCommand.Execute(null);
    }

    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        vm.DisconnectCommand.Execute(null);
    }

    private void Button_Click_3(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        vm.PairDeviceCommand.Execute(null);
    }

    private void ItemsControl_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        svCommands.ScrollToEnd();
    }
}
