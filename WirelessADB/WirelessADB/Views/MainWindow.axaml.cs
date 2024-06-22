﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using WirelessADB.ViewModels;

namespace WirelessADB.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        DataContext = new MainViewModel
        {
            Window = this
        };
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        (DataContext as MainViewModel).SaveSettings();
        base.OnClosing(e);
    }

    private void OpenUrl_Click(object sender, RoutedEventArgs e)
    {
        // URL a la que se quiere navegar
        var url = "https://www.youtube.com/channel/UCWhJjCN3ICLXmiOWleqymEg";

        // Abrir la URL en el navegador predeterminado
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    private void OpenUrl2_Click(object sender, RoutedEventArgs e)
    {
        // URL a la que se quiere navegar
        var url = "https://www.example.com";

        // Abrir la URL en el navegador predeterminado
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}