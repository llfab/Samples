using System;
using System.Collections.Generic;
using System.Text;
using LibVLCSharp;
using LibVLCSharp.Shared;
using BitsOfNature.UI.Avalonia;
using Avalonia.Controls;
using BitsOfNature.Core.IO.Tracing;

namespace CrossPlatformApp.ViewModels
{
    public class VideoWindowViewModel : CrossPlatformApp.Mvvm.ViewModelBase, IDisposable
    {
        private Trace trace;

        public string Greeting => "Welcome to Avalonia!";

        private XrayViewModel xrayModel;
        public XrayViewModel XrayModel
        {
            get { return xrayModel; }
        }

        private readonly LibVLC libVlc;
        private MediaPlayer mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => mediaPlayer;
        }

        static VideoWindowViewModel()
        {
            LibVLCSharp.Shared.Core.Initialize();
        }

        public VideoWindowViewModel()
        {
            xrayModel = new XrayViewModel();

            libVlc = new LibVLC();
            mediaPlayer = new MediaPlayer(libVlc);

            if (!Design.IsDesignMode)
            {
                trace = new Trace("VideoWindowViewModel");
            }
        }

        public void Dispose()
        {
            try
            {
                mediaPlayer?.Dispose();
                libVlc?.Dispose();
            }
            catch (Exception ex)
            {
                trace.Error("Dispose(): Error.\n{0}", ex);
            }
        }

        public void Play()
        {
            using (var media = new Media(libVlc, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")))
            {
                mediaPlayer.Play(media);
            }
        }

        public void Stop()
        {
            mediaPlayer.Stop();
        }
    }
}
