using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;
using Avalonia.Threading;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.Measurements;
using BitsOfNature.Rendering.Vulkan.Engine;

namespace CrossPlatformAppVk.Views
{
    public partial class MainWindow : Window
    {
        private DateTime _start;
        private VulkanPageControl _vulkan1;
        private VulkanPageControl _vulkan2;
        private SceneCamera _camera1;
        private SceneCamera _camera2;
        private FpsCounter _fps;
        private int _updateFpsCounter = 0;

        private bool _magnify;
        private bool _secondaryRendererEnabled = true;
        private bool _renderBenchmarkRunning;
        private bool _useSecondaryDevice;

        // Magnifier Image Settings
        private const double MagImageScale = 1.25; // Scale of image to magnified ellipse
        private const double MagImageOffset = 0.12; // Offset of magnified ellipse within image

        // Magnifier Settings (filled by default slider vlaues)
        private double _magSize = 100;
        private double _magScale = 2;

        private Ellipse _magEllipse;
        private Slider _scaleSlider;
        private Slider _sizeSlider;
        private Image _magImage;
        private RadioButton _magBox;
        private Grid _host;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this._vulkan1 = this.FindControl<VulkanPageControl>("Vulkan1");
            this._vulkan2 = this.FindControl<VulkanPageControl>("Vulkan2");
            this._magEllipse = this.FindControl<Ellipse>("MagEllipse");
            this._scaleSlider = this.FindControl<Slider>("Scale");
            this._sizeSlider = this.FindControl<Slider>("Size");
            this._magImage = this.FindControl<Image>("MagImage");
            this._magBox = this.FindControl<RadioButton>("MagBox");
            this._host = this.FindControl<Grid>("Host");
            _magEllipse.Height = this._magSize;
            _magEllipse.Width = this._magSize;
            _scaleSlider.Value = this._magScale;

            this._magBox.Checked += this.MagBox_Checked;
            this._magBox.Unchecked += this.MagBox_Unchecked;
            this._scaleSlider.PointerMoved += delegate(object sender, PointerEventArgs args)
            {
                if (_magnify)
                {
                    this._magScale = this._scaleSlider.Value;
                }
            };
            this._sizeSlider.PointerMoved += delegate (object sender, PointerEventArgs args)
            {
                if (_magnify)
                {
                    this._magSize = this._sizeSlider.Value;
                    this._magImage.Width = this._magSize * MagImageScale;

                    // Setup the Magnifier Size
                    this._magEllipse.Height = this._magSize;
                    this._magEllipse.Width = this._magSize;
                    this._magImage.InvalidateVisual();
                    this._magEllipse.InvalidateVisual();
                }
            };

            this._host.PointerMoved += MagElement_MouseMove;
            this._magEllipse.PointerMoved += MagElement_MouseMove;
            this._magImage.PointerMoved += MagElement_MouseMove;

            this._magEllipse.Height = this._magSize;
            this._magEllipse.Width = this._magSize;
            this._magEllipse.IsVisible = false;
            this._magImage.Width = this._magSize * MagImageScale;
            this._magImage.IsVisible = false;

            AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60));
            DefaultRenderTimer defaultRenderTimer = AvaloniaLocator.CurrentMutable.GetService<IRenderTimer>() as DefaultRenderTimer;
            TraceApplication.Trace.Info("MainWindow: Rendertime is set to {0} fps", defaultRenderTimer.FramesPerSecond);
        }

        private void MagElement_MouseMove(object sender, PointerEventArgs e)
        {
            if (this._magnify)
            {
                Point point = e.GetPosition(this._host); 
                if (!(point.X < 0 || point.Y < 0 || point.X > this._host.Width || point.Y > this._host.Height))
                {
                    Canvas.SetTop(this._magEllipse, point.Y - (this._magSize / 2));
                    Canvas.SetLeft(this._magEllipse, point.X - (this._magSize / 2));

                    Canvas.SetTop(this._magImage, point.Y - (this._magSize * (.5 + MagImageOffset)));
                    Canvas.SetLeft(this._magImage, point.X - (this._magSize * (.5 + MagImageOffset)));
                    this._magImage.Width = this._magSize * MagImageScale;

                    this._magEllipse.IsVisible = true;
                    this._magImage.IsVisible = true;
                    this._magImage.InvalidateVisual();
                }
                else
                {
                    this._magEllipse.IsVisible = false;
                    this._magImage.IsVisible = false;
                }
            }

            if (!this._renderBenchmarkRunning)
            {
                this.Title = this._vulkan1.Info;
            }


        }
        private void MagBox_Checked(object sender, RoutedEventArgs e)
        {
            this._magnify = false;
            this._magEllipse.IsVisible = false;
            this._magImage.IsVisible = false;
            this._host.InvalidateVisual();
            this._vulkan1.InvalidateVisual();
        }

        private void MagBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this._magnify = true;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan duration = TimeSpan.FromSeconds(5);
            TimeSpan elapsed = DateTime.Now - _start;
            if (elapsed > duration)
            {
                DispatcherTimer renderTimer = sender as DispatcherTimer;
                renderTimer.Stop();
                if (this._secondaryRendererEnabled)
                {
                    this._vulkan2.Camera = _camera2;
                    this._vulkan2.InvalidateVisual();
                }
                this._vulkan1.Camera = _camera1;
                this._renderBenchmarkRunning = false;

                this._vulkan1.InvalidateVisual();
                return;
            }

            double angle = ((elapsed.TotalSeconds % duration.TotalSeconds) / duration.TotalSeconds) * 8 * Math.PI;
            Pose3D rot = Pose3D.CreateRotation(this._camera1.ViewCenter, this._camera1.UpDirection, angle);
            Pose3D rot2 = Pose3D.CreateRotation(this._camera2.ViewCenter, this._camera2.UpDirection, -angle);
            this._vulkan1.Camera = rot * _camera1;
            if (this._secondaryRendererEnabled)
            {
                this._vulkan2.Camera = rot2 * _camera2;
                this._vulkan2.InvalidateVisual();
            }
            this._fps.Update();
            if (this._updateFpsCounter++ % 10 == 0)
            {
                string secondRendererTxt = this._secondaryRendererEnabled ? "on" : "off";
                string usageDeviceTxt = this._useSecondaryDevice ? "Bits device" : "Avalonia device";
                this.Title =
                    $"{_fps.Fps:F1} FPS, MainWindow: {this._vulkan1.Bounds.Size.Width} x {this._vulkan1.Bounds.Size.Height}, " +
                    $"second Renderer: {secondRendererTxt}, Device usage: {usageDeviceTxt}";
            }
            this._vulkan1.InvalidateVisual();

        }


        private void BtnRenderLoop(object sender, RoutedEventArgs e)
        {
            if (this._camera1 == null && this._camera2 == null)
            {
                this._camera1 = this._vulkan1.Camera.Clone();
                this._camera2 = this._vulkan2.Camera.Clone();
            }
            this._start = DateTime.Now;
            this._fps = new FpsCounter(TimeSpan.FromSeconds(1));

            DispatcherTimer renderTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(0)
            };
            this._renderBenchmarkRunning = true;
            renderTimer.Tick += DispatcherTimer_Tick;
            renderTimer.Start();
        }

        private void RbtnAllRenderer(object sender, RoutedEventArgs e)
        {
            _secondaryRendererEnabled = true;
        }
        private void RbtnSingleRenderer(object sender, RoutedEventArgs e)
        {
            _secondaryRendererEnabled = false;
        }

        private void RbtnSingleDevice(object sender, RoutedEventArgs e)
        {
            if (this._useSecondaryDevice)
            {
                this._useSecondaryDevice = false;
                this._vulkan1.UseSecondaryDevice = this._useSecondaryDevice;
                this._vulkan2.UseSecondaryDevice = this._useSecondaryDevice;
            }
        }
        private void RbtnSecondDevice(object sender, RoutedEventArgs e)
        {
            if (!this._useSecondaryDevice)
            {
                this._useSecondaryDevice = true;
                this._vulkan1.UseSecondaryDevice = this._useSecondaryDevice;
                this._vulkan2.UseSecondaryDevice = this._useSecondaryDevice;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

    }
}
