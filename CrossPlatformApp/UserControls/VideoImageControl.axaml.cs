// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using BitsOfNature.Core;
using BitsOfNature.Core.Imaging;
using BitsOfNature.Core.IO.Tracing;

namespace CrossPlatformApp.UserControls
{
    public partial class VideoImageControl : UserControl, IDisposable
    {
        #region Fields

        private Trace trace;

        private ArgbImage imageSource;
        public ArgbImage ImageSource
        {
            get => imageSource;
            //set => SetAndRaise(ImageProperty, ref image, value);
            set
            {
                imageSource = value;
                frameCount++;
                UpdateImage();
            }
        }

        public static readonly DirectProperty<VideoImageControl, ArgbImage> ImageSourceProperty =
           AvaloniaProperty.RegisterDirect<VideoImageControl, ArgbImage>("ImageSource", o => o.ImageSource, (o, v) => o.ImageSource = v);

        private WriteableBitmap bitmapContent;
        public WriteableBitmap BitmapContent
        {
            get => bitmapContent;
            set => SetAndRaise(BitmapContentProperty, ref bitmapContent, value);
        }

        public static readonly DirectProperty<VideoImageControl, WriteableBitmap> BitmapContentProperty =
           AvaloniaProperty.RegisterDirect<VideoImageControl, WriteableBitmap>("BitmapContent", o => o.BitmapContent, (o, v) => o.BitmapContent = v);

        private int frameCount;
        private DispatcherTimer fpsTimer;
        private DateTime fpsResetTimestamp;

        private double fps;
        public double Fps
        {
            get => fps;
            set => SetAndRaise(FpsProperty, ref fps, value);
        }

        public static readonly DirectProperty<VideoImageControl, double> FpsProperty =
           AvaloniaProperty.RegisterDirect<VideoImageControl, double>("Fps", o => o.Fps, (o, v) => o.Fps = v);

        private Image imageControl;

        #endregion

        #region Construction / Destruction

        public VideoImageControl()
        {
            trace = new Trace("VideoImageControl");

            InitializeComponent();

            imageControl = this.FindControl<Image>("ImageDisplay");

            frameCount = 0;
            fpsResetTimestamp = DateTime.UtcNow;
            fps = 0;

            fpsTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 2500), DispatcherPriority.Background, OnFpsTimerTick);

            if (!Design.IsDesignMode)
            {
                fpsTimer.Start();
            }
        }

        public void Dispose()
        {
            if (fpsTimer != null)
            {
                fpsTimer.Stop();
                fpsTimer = null;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        #endregion

        #region Private Helpers

        private void UpdateImage()
        {
            try
            {
                if (imageSource == null)
                {
                    bitmapContent = null;
                    return;
                }

                if (bitmapContent == null || bitmapContent.PixelSize.Width != imageSource.Width || bitmapContent.PixelSize.Height != imageSource.Height)
                {
                    if (bitmapContent != null)
                    {
                        bitmapContent.Dispose();
                        bitmapContent = null;
                    }

                    //await Dispatcher.UIThread.InvokeAsync(new Action(() => { BitmapContent = new WriteableBitmap(new PixelSize(imageSource.Width, imageSource.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul); }));
                    BitmapContent = new WriteableBitmap(new PixelSize(imageSource.Width, imageSource.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
                }

#pragma warning disable CS8604 // Possible null reference argument.
                WriteImageToWriteableBitmap(imageSource, bitmapContent);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            catch (Exception ex)
            {
                trace.Error("UpdateImage(): Error.\n{0}", ex);
            }
            finally
            {
                try
                {
                    Dispatcher.UIThread.Post(imageControl.InvalidateVisual, DispatcherPriority.Background);
                }
                catch (Exception ex)
                {
                    trace.Error("UpdateImage(): Error dispatching change.\n{0}", ex);

                }
            }
        }

        private static unsafe void WriteImageToWriteableBitmap(ArgbImage image, WriteableBitmap bitmap)
        {
            using (ILockedFramebuffer buf = bitmap.Lock())
            {
                uint* ptarget = (uint*)buf.Address;
                fixed (uint* psource = image.Pixels)
                {
                    int count = image.Size.XY * sizeof(uint);
                    Buffer.MemoryCopy(psource, ptarget, count, count);
                }
            }
        }

        #endregion

        #region Callbacks

        private void OnFpsTimerTick(object sender, EventArgs e)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                double fpsDurationSeconds = (now - fpsResetTimestamp).TotalSeconds;
                Fps = (double)frameCount / fpsDurationSeconds;
                fpsResetTimestamp = now;
                frameCount = 0;
            }
            catch (Exception ex)
            {
                trace.Error("OnFpsTimerTick(): Error.\n{0}", ex);
            }
        }

        #endregion
    }
}
