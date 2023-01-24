using System;
using System.Collections.Generic;
using System.Text;
using BitsOfNature.UI.Avalonia;
using Avalonia.Controls;
using BitsOfNature.Core.IO.Tracing;
using CrossPlatformApp.Views;
using CrossPlatformApp.Infrastructure;
using BitsOfNature.UI.Avalonia.Mvvm;

namespace CrossPlatformApp.ViewModels
{
    public class MainWindowViewModel : CrossPlatformApp.Mvvm.ViewModelBase, IDisposable
    {
        private Trace trace;

        public string Greeting => "Welcome to Avalonia!";

        private XrayViewModel xrayModel;
        public XrayViewModel XrayModel
        {
            get { return xrayModel; }
        }

        private VideoWindow videoWindow;
        public VideoWindow VideoWindow
        {
            get { return videoWindow; }
        }

        private VideoWindowViewModel videoModel;
        public VideoWindowViewModel VideoModel
        {
            get { return videoModel; }
        }

        private PelvisLiveView pelvisLiveView;
        public PelvisLiveView PelvisLiveView
        {
            get { return pelvisLiveView; }
        }

        private PelvisLiveViewModel pelvisLiveModel;
        public PelvisLiveViewModel PelvisLiveModel
        {
            get { return pelvisLiveModel; }
        }

        private bool isVideoVisible;
        public bool IsVideoVisible
        {
            get => isVideoVisible;
            set 
            {
                if (isVideoVisible != value)
                {
                    RaiseAndSetIfChanged(ref isVideoVisible, value);
                    if (value)
                        ShowVideo();
                    else
                        HideVideo();
                }
            }
        }

        public SimpleCommand StartPelvisSiLive { get; private set; }
        public SimpleCommand StopPelvisSiLive { get; private set; }

        public MainWindowViewModel()
        {
            xrayModel = new XrayViewModel();
            pelvisLiveModel = new PelvisLiveViewModel();
            isVideoVisible = false;

            if (!Design.IsDesignMode)
            {
                trace = new Trace("MainWindowViewModel");

                StartPelvisSiLive = SimpleCommand.Create(() => ShowPelvisSiLIve());
                StopPelvisSiLive = SimpleCommand.Create(() => HidePelvisSiLIve());
            }
        }

        public void Dispose()
        {
            try
            {
                trace.Info("Dispose(): called");

                HideVideo();
            }
            catch (Exception ex)
            {
                trace.Error("Dispose(): Error.\n{0}", ex);
            }
        }

        public void Closing()
        {
            Dispose();
        }

        private void ShowVideo()
        {
            MainWindow mainWindow = CrossPlatformAppSystem.Instance.Services.Get<MainWindow>();

            if (videoModel == null)
            {
                videoModel = new VideoWindowViewModel();
            }

            videoWindow = new VideoWindow()
            {
                DataContext = videoModel,
            };
            videoWindow.Position = mainWindow.Position + new Avalonia.PixelPoint((int)(mainWindow.ClientSize.Width - videoWindow.Width), (int)(mainWindow.ClientSize.Height - videoWindow.Height + 20));

            videoWindow.Show();
        }

        private void HideVideo()
        {
            if (videoWindow != null)
            {
                videoWindow.Hide();
                videoWindow.Close();
                videoWindow = null;
            }
        }

        private void ShowPelvisSiLIve()
        {
            pelvisLiveView = new PelvisLiveView()
            {
                DataContext = pelvisLiveModel,
            };

            pelvisLiveView.Show();
        }

        private void HidePelvisSiLIve()
        {
            if (pelvisLiveView != null)
            {
                pelvisLiveView.Hide();
                pelvisLiveView.Close();
                pelvisLiveView = null;
            }
        }


    }
}
