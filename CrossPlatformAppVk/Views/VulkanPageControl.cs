using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Vulkan.Controls;
using Avalonia.Vulkan;
using Avalonia;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Grid;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.UI.Avalonia.Input;
using BitsOfNature.UI.Avalonia.Manipulation;
using BitsOfNature.UI.Avalonia.Rendering;
using BitsOfNature.Core.IO.Tracing;

namespace CrossPlatformAppVk.Views
{
    public class VulkanPageControl : VulkanControlBase, ISceneViewerControl
    {
        private AvaloniaRendererBase _renderer;
        private bool _useSecondaryDevice;

        public bool UseSecondaryDevice
        {
            get { return _useSecondaryDevice;  }
            set
            {
                if (value != this._useSecondaryDevice)
                {
                    _useSecondaryDevice = value;
                    this._renderer = null;
                    this.InvalidateVisual();
                }
            }
        }
        private VulkanPlatformInterface _platformInterface;
        public SceneCamera Camera { get; set; }

        public GridRect Viewport
        {
            get
            {
                return new GridRect(0, 0, (int)Bounds.Width, (int)Bounds.Height);
            }
        }

        private string _info;

        private static readonly DirectProperty<VulkanPageControl, string> s_infoProperty =
            AvaloniaProperty.RegisterDirect<VulkanPageControl, string>("Info", o => o.Info, (o, v) => o.Info = v);

        public string Info
        {
            get => _info;
            private set => SetAndRaise(s_infoProperty, ref _info, value);
        }

        static VulkanPageControl()
        {
        }

        protected override void OnVulkanRender(VulkanPlatformInterface platformInterface, VulkanImageInfo info)
        {
            if (this._renderer == null || info.PixelSize.Width != this._renderer.RenderTarget.Size.Width ||
                info.PixelSize.Height != this._renderer.RenderTarget.Size.Height)
            {
                this._renderer = null;
                if (this._useSecondaryDevice)
                {

                    SwitchToSecondDevice(platformInterface, info);
                }
                else
                {
                    SwitchToSingleDevice(platformInterface, info);
                }
            }
            this._renderer.Render(this.Camera, new Int2(info.PixelSize.Width, info.PixelSize.Height));
        }


        protected override void OnVulkanInit(VulkanPlatformInterface platformInterface, VulkanImageInfo info)
        {
            base.OnVulkanInit(platformInterface, info);
            this._platformInterface = platformInterface;

            if (this._useSecondaryDevice)
            {
                SwitchToSecondDevice(platformInterface, info);
            }
            else
            {
                SwitchToSingleDevice(platformInterface, info);
            }

            Camera = new SceneCamera();
            Camera.LookAtOrtho(Box3D.CreateCenterBox(new Point3D(0, 0, 0), 180, 120, 2048));
        }

        private void SwitchToSingleDevice(VulkanPlatformInterface platformInterface, VulkanImageInfo info)
        {
            if (this._renderer is null or AvaloniaRendererSecondDevice)
            {
                TraceApplication.Trace.Info("VulkanPageControl: Use single vulkan device, created by Avalonia.");
                AvaloniaRendererBase.CreationInfo createInfo = new AvaloniaRendererBase.CreationInfo()
                {
                    InstanceHandle = (ulong)platformInterface.Instance.Handle,
                    PhysicalDeviceHandle = (ulong)platformInterface.PhysicalDevice.Handle,
                    DeviceHandle = (ulong)platformInterface.Device.Handle,
                    PixelSize = new Int2(info.PixelSize.Width, info.PixelSize.Height),
                    AvaloniaImage = info.Image
                };
                this._renderer = new AvaloniaRendererSingleDevice(createInfo);

                string deviceName = this._platformInterface.PhysicalDevice.DeviceName;
                Version version = this._platformInterface.PhysicalDevice.ApiVersion;
                Info = $"Renderer: {deviceName} Version: {version.Major}.{version.Minor}.{version.Revision}, Device usage: single (Avalonia device)";

            }
        }
        private void SwitchToSecondDevice(VulkanPlatformInterface platformInterface, VulkanImageInfo info)
        {
            if (this._renderer is null or AvaloniaRendererSingleDevice)
            {
                TraceApplication.Trace.Info("VulkanPageControl: Use second vulkan device, created by BitsOfNature.");
                AvaloniaRendererBase.CreationInfo createInfo = new AvaloniaRendererBase.CreationInfo()
                {
                    InstanceHandle = (ulong)platformInterface.Instance.Handle,
                    PhysicalDeviceHandle = (ulong)platformInterface.PhysicalDevice.Handle,
                    DeviceHandle = (ulong)platformInterface.Device.Handle,
                    PixelSize = new Int2(info.PixelSize.Width, info.PixelSize.Height),
                    AvaloniaImage = info.Image
                };
                this._renderer = new AvaloniaRendererSecondDevice(createInfo);

                string deviceName = this._platformInterface.PhysicalDevice.DeviceName;
                Version version = this._platformInterface.PhysicalDevice.ApiVersion;
                Info = $"Renderer: {deviceName} Version: {version.Major}.{version.Minor}.{version.Revision}, Device usage: second (Bits device)";
            }
        }

        public VulkanPageControl()
        {
            if (!Design.IsDesignMode)
            {
                MouseAdapter mouseAdapter = new(this);
                mouseAdapter.Controllers.Add(new CameraMouseController(this, KeyModifiers.Control));
                mouseAdapter.Activate();
            }
        }

    }
}
