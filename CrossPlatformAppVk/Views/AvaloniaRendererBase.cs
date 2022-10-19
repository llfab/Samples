// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using BitsOfNature.Core.Utils;
using Avalonia.Vulkan;

using BitsVulkan = BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsVulkanEngine = BitsOfNature.Rendering.Vulkan.Engine;

namespace CrossPlatformAppVk.Views
{
    public abstract class AvaloniaRendererBase
    {
        public struct CreationInfo
        {
            public ulong InstanceHandle = default;
            public ulong PhysicalDeviceHandle = default;
            public ulong DeviceHandle = default;
            public Int2 PixelSize = new Int2();
            public VulkanImage AvaloniaImage = default;
            public CreationInfo() {}
        }

        protected CreationInfo _creationInfo;
        
        protected AvaloniaRenderTarget _renderTarget;
        public AvaloniaRenderTarget RenderTarget => _renderTarget;
        protected TestRenderer _testRenderer;
        protected BitsVulkanEngine.RenderingContext _renderContext;
        protected BitsVulkan.Device _avaloniaDevice;

        protected AvaloniaRendererBase(CreationInfo creationInfo)
        {
            this._creationInfo = creationInfo;
        }

        public abstract void Init();

        public abstract void Render(BitsVulkanEngine.SceneCamera camera, Int2 pixelSize);
    }
}
