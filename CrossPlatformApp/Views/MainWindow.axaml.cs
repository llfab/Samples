using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.UI.Avalonia.Localization;
using CrossPlatformApp.ViewModels;

namespace CrossPlatformApp.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!Design.IsDesignMode)
            {
                var vm = DataContext as MainWindowViewModel;
                vm?.Closing();

                try
                {
                    //this.Clock.PlayState = Avalonia.Animation.PlayState.Stop;
                    //Button button = this.FindControl<Button>("AnimButton");
                    //button.Clock.PlayState = Avalonia.Animation.PlayState.Stop;
                }
                catch (Exception ex)
                {
                    TraceApplication.Trace.Error("OnClosing(): Error.\n{0}", ex);
                }
            }
        }

        private void OnLanguageChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!Design.IsDesignMode)
            {
                var cb = sender as ComboBox;
                if (cb == null) return;
                var language = cb.SelectedIndex == 0 ? "en-US" : "de-DE";
                Localizer.Instance.LoadLanguage(language, "avares://CrossPlatformApp/Assets/i18n");
            }
        } 
    }
}
