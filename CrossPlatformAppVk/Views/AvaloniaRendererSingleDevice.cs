// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2022 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using BitsOfNature.Core;
using BitsOfNature.Rendering.Vulkan.Interop;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsOfNature.Rendering.Vulkan.Memory;
using BitsVulkan = BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsVulkanEngine = BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.Utils;
using BitsOfNature.Core.Grid;

namespace CrossPlatformAppVk.Views
{
    public class AvaloniaRendererSingleDevice : AvaloniaRendererBase
    {

        private static unsafe BitsVulkan.Device AcquireDevice(BitsVulkan.VulkanInstance instance, BitsVulkan.PhysicalDevice physicalDevice, IntPtr deviceHandle)
        {
            BitsVulkan.Device device = new BitsVulkan.Device(new VkDevice((ulong)deviceHandle), instance, physicalDevice);
            DeviceBuilder deviceBuilder = new DeviceBuilder(instance);
            deviceBuilder.ConfigureOpenGLInterop();
            deviceBuilder.ConfigureSwapChain();

            Trace trace = Trace.ForType<BitsVulkan.DeviceBuilder>();

            trace.Info("{0}(): Using physical device[{1}] with driver version[{2}]", trace.CallSite(), physicalDevice.DeviceName,
                physicalDevice.DriverVersion.VersionStringMajorMinor);

            HashSet<string> availableExtensions = physicalDevice.GetExtensionNames().ToHashSet();
            trace.Debug("{0}(): Extensions available from physical device[{1}]", trace.CallSite(),
                string.Join(", ", availableExtensions));


            List<(int index, VkQueueFlags flags)> queueFamilies = new List<(int family, VkQueueFlags flags)>();

            {
                int index = physicalDevice.FindQueueFamily(VkQueueFlags.Graphics, true) ?? -1;
                Assert.That(index >= 0, "Could not find queue family for graphics on device {0}", physicalDevice.DeviceName);
                queueFamilies.Add((index, VkQueueFlags.Graphics));
            }

            VkDeviceQueueCreateInfo[] queueCreateInfos = new VkDeviceQueueCreateInfo[queueFamilies.Count];

            float queuePriority = 1.0f;
            for (int i = 0; i < queueCreateInfos.Length; i++)
            {
                queueCreateInfos[i] = new VkDeviceQueueCreateInfo()
                {
                    QueueFamilyIndex = (uint)queueFamilies[i].index,
                    QueueCount = 1,
                    pQueuePriorities = &queuePriority
                };
            }

            List<DeviceQueue> queues = new List<DeviceQueue>();

            foreach ((int index, VkQueueFlags flags) in queueFamilies)
            {
                queues.Add(new DeviceQueue(device, index, flags, true && flags.HasFlag(VkQueueFlags.Graphics)));
            }

            NaiveDeviceMemoryAllocator memoryAllocator = new NaiveDeviceMemoryAllocator();
            device.Init(queues.ToArray(), memoryAllocator, null);

            return device;
        }

        public AvaloniaRendererSingleDevice(CreationInfo creationInfo) : base(creationInfo)
        {
            Init();
        }

        /// <inheritdoc />
        public override void Init()
        {
            BitsVulkan.VulkanInstance instance = new BitsVulkan.VulkanInstance(new VkInstance(this._creationInfo.InstanceHandle), new HashSet<string>(), new HashSet<string>());
            BitsVulkan.PhysicalDevice physicalDevice = new BitsVulkan.PhysicalDevice(instance, new VkPhysicalDevice(this._creationInfo.PhysicalDeviceHandle));
            this._avaloniaDevice = AcquireDevice(instance, physicalDevice, (IntPtr)this._creationInfo.DeviceHandle);

            this._renderContext = new BitsVulkanEngine.RenderingContext(this._avaloniaDevice);

            this._renderTarget = new AvaloniaRenderTarget(_renderContext, this._avaloniaDevice, this._creationInfo.PixelSize, this._creationInfo.AvaloniaImage, true);
            this._testRenderer = new TestRenderer(_renderContext);

        }

        /// <inheritdoc />
        public override void Render(BitsVulkanEngine.SceneCamera camera, Int2 pixelSize)
        {
            if (this._renderTarget.Size != pixelSize)
            {
                this._renderTarget.Dispose();
                this._renderTarget = new AvaloniaRenderTarget(this._renderContext, this._avaloniaDevice, pixelSize, this._creationInfo.AvaloniaImage, true);
            }
            this._testRenderer.Render(this._renderTarget, camera, GridRect.FromSize(this._renderTarget.Size));
            this._renderTarget.Device.WaitIdle();
        }
    }
}
