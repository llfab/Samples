using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.ViewModels;
using CrossPlatformApp.Views;
using CrossPlatformApp.Infrastructure;

namespace CrossPlatformApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainWindow mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow = mainWindow;
                
                CrossPlatformAppSystem.Instance.Services.Register(mainWindow);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
