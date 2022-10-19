using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Vulkan.Controls;
using Avalonia.Vulkan;
using Avalonia;
using BitsOfNature.Core;
using BitsOfNature.Core.Geometry3D;
using BitsOfNature.Core.Grid;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsOfNature.UI.Avalonia.Input;
using BitsOfNature.UI.Avalonia.Manipulation;
using BitsOfNature.UI.Avalonia.Rendering;
using BitsVulkan = BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsVulkanEngine = BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Core.IO.Tracing;
using System;

namespace CrossPlatformAppVk.Views
{
    public class VulkanPageControl : VulkanControlBase, ISceneViewerControl
    {
        private SceneCamera _camera;
        private readonly MouseAdapter _mouseAdapter;
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
        public SceneCamera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }
        public GridRect Viewport
        {
            get
            {
                return new GridRect(0, 0, (int)Bounds.Width, (int)Bounds.Height);
            }
        }

        private string _info;

        public static readonly DirectProperty<VulkanPageControl, string> InfoProperty =
            AvaloniaProperty.RegisterDirect<VulkanPageControl, string>("Info", o => o.Info, (o, v) => o.Info = v);

        public string Info
        {
            get => _info;
            private set => SetAndRaise(InfoProperty, ref _info, value);
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
            this._renderer.Render(this._camera, new Int2(info.PixelSize.Width, info.PixelSize.Height));
        }

        protected unsafe override void OnVulkanDeinit(VulkanPlatformInterface platformInterface, VulkanImageInfo info)
        {
            base.OnVulkanDeinit(platformInterface, info);
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

            _camera = new SceneCamera();
            _camera.LookAtOrtho(Box3D.CreateCenterBox(new Point3D(0, 0, 0), 180, 120, 2048));

            var deviceName = platformInterface.PhysicalDevice.DeviceName;
            var version = platformInterface.PhysicalDevice.ApiVersion;

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

                var deviceName = this._platformInterface.PhysicalDevice.DeviceName;
                var version = this._platformInterface.PhysicalDevice.ApiVersion;
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

                var deviceName = this._platformInterface.PhysicalDevice.DeviceName;
                var version = this._platformInterface.PhysicalDevice.ApiVersion;
                Info = $"Renderer: {deviceName} Version: {version.Major}.{version.Minor}.{version.Revision}, Device usage: second (Bits device)";
            }
        }

        public VulkanPageControl()
        {
            if (!Design.IsDesignMode)
            {
                _mouseAdapter = new MouseAdapter(this);
                _mouseAdapter.Controllers.Add(new CameraMouseController(this, KeyModifiers.Control));
                _mouseAdapter.Activate();
            }
        }

    }
}
