// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using BitsOfNature.Core.Grid;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsVulkan = BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsVulkanEngine = BitsOfNature.Rendering.Vulkan.Engine;

namespace CrossPlatformAppVk.Views
{
    public class AvaloniaRendererSecondDevice : AvaloniaRendererBase
    {
        public AvaloniaRendererSecondDevice(CreationInfo creationInfo) : base(creationInfo)
        {
            Init();
        }

        /// <inheritdoc />
        public override void Init()
        {
            BitsVulkan.VulkanInstance instance = BitsVulkan.VulkanInstance.
                New().
                SetApplication("VulkanTest", new VersionInfo(0, 0, 1)).
                EnableDebug(true, VulkanDebugCallback.Trace).
                ConfigureOpenGLInterop().
                ConfigureSwapChain(). 
                Build();

            this._avaloniaDevice = BitsVulkan.Device.New(instance).
                SetPhysicalDevice(DeviceSelector.Default).
                ConfigureOpenGLInterop().
                ConfigureSwapChain(). 
                Build();
            this._renderContext = new BitsVulkanEngine.RenderingContext(this._avaloniaDevice);

            this._renderTarget = new AvaloniaRenderTarget(_renderContext, this._avaloniaDevice, this._creationInfo.PixelSize, this._creationInfo.AvaloniaImage, false);
            this._testRenderer = new TestRenderer(_renderContext);

        }

        /// <inheritdoc />
        public override void Render(SceneCamera camera, Int2 pixelSize)
        {
            if (this._renderTarget.Size != pixelSize)
            {
                this._renderTarget.Dispose();
                this._renderTarget = new AvaloniaRenderTarget(this._renderContext, this._avaloniaDevice, pixelSize, this._creationInfo.AvaloniaImage, false);
            }
            this._testRenderer.Render(this._renderTarget, camera, GridRect.FromSize(this._renderTarget.Size));
            this._renderTarget.Device.WaitIdle();
        }
    }
}
