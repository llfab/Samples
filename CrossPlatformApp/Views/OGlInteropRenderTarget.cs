// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//  
//  Copyright (c) 2021 Stryker
// ===========================================================================

using System;
using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.Interop;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsOfNature.Rendering.Vulkan.Interop.Win32;

namespace CrossPlatformApp.Views
{
    public unsafe class OGlInteropRenderTarget : RenderTarget
    {
        private VkSemaphore glReadySemaphore;
        private HANDLE glReadyHandle;
        private int glReadyLinuxHandle;
        private VkSemaphore glCompleteSemaphore;
        private HANDLE glCompleteHandle;
        private int glCompleteLinuxHandle;

        public HANDLE GlReadyHandle => glReadyHandle;
        public int GlReadyLinuxHandle => glReadyLinuxHandle;
        private VkSemaphore GlCompleteSemaphore => glCompleteSemaphore;
        public HANDLE GlCompleteHandle => glCompleteHandle;
        public int GlCompleteLinuxHandle => glCompleteLinuxHandle;
        private VkSemaphore GlReadySemaphore => glReadySemaphore;


        private Image renderImage;
        private ImageView renderImageView;
        private Image sharedImage;
        private ImageView sharedImageView;
        private DepthBuffer depthBuffer;
        private ImageView depthBufferView;

        private void* rawScreenbuffer;
        public void* RawScreenBuffer => rawScreenbuffer;

        private bool memoryMapped = false;

        private UInt64 memoryImageShareSize = 0;
        public UInt64 MemoryImageShareSize => memoryImageShareSize;

        private HANDLE sharedMemoryHandle;
        private int sharedLinuxMemoryHandle;
        public HANDLE SharedMemoryHandle => sharedMemoryHandle;
        public int SharedLinuxMemoryHandle => sharedLinuxMemoryHandle;

        private readonly VkSampleCountFlags sampleCountFlags;

        private BitsOfNature.Rendering.Vulkan.LowLevel.Buffer offscreenBuffer;

        private FrameBuffer frameBuffer;
        private readonly RenderPass renderPass;
        private Int2 imageDimension;
        public Int2 ImageDimension => imageDimension;
        private readonly Device device;

        private readonly bool useSharedTexture;
        public bool UseSharedTexture => useSharedTexture;

        public override FrameBuffer FrameBuffer => frameBuffer;
        public override Int2 Size => imageDimension;

        public override bool HasSwapChain => false;

        private void PresentCopiedTexture(CommandBuffer cmd, Fence renderFinishedFence)
        {
            VkResult result;
            VkBufferImageCopy bufferImageCopy = new VkBufferImageCopy
            {
                bufferOffset = 0,
                bufferRowLength = 0, // tightly packed
                bufferImageHeight = 0, // tightly packed
                imageSubresource = new VkImageSubresourceLayers { aspectMask = VkImageAspectFlags.Color, mipLevel = 0, baseArrayLayer = 0, layerCount = 1 },
                imageOffset = new VkOffset3D { x = 0, y = 0, z = 0 },
                imageExtent = new VkExtent3D { width = (uint)imageDimension.Width, height = (uint)imageDimension.Height, depth = 1 },
            };

            VkBufferMemoryBarrier transferBarrier = new VkBufferMemoryBarrier
            {
                pNext = null,
                srcAccessMask = VkAccessFlags.TransferWrite,
                dstAccessMask = VkAccessFlags.MemoryWrite,
                srcQueueFamilyIndex = VulkanConstants.QueueFamilyIgnored,
                dstQueueFamilyIndex = VulkanConstants.QueueFamilyIgnored,
                buffer = offscreenBuffer.Handle,
                offset = 0,
                size = VulkanConstants.WholeSize,
            };
            device.Api.vkCmdCopyImageToBuffer(cmd.Handle, sharedImage.Handle, VkImageLayout.TransferSrcOptimal, offscreenBuffer.Handle, 1, &bufferImageCopy);
            device.Api.vkCmdPipelineBarrier(cmd.Handle, VkPipelineStageFlags.Transfer, VkPipelineStageFlags.Host, 0, 0, null, 1, &transferBarrier, 0, null);
            // Must called after vkCmdCopyImageToBuffer!
            cmd.End();

            void* rawBuffer;
            result = device.Api.vkMapMemory(device.Handle, offscreenBuffer.Memory.Handle, 0, VulkanConstants.WholeSize, 0, &rawBuffer);
            result.AssertOk();
            this.rawScreenbuffer = rawBuffer;


            VkSubmitInfo submitInfo;
            VkCommandBuffer[] commandBuffers = new VkCommandBuffer[] { cmd.Handle };

            fixed (VkCommandBuffer* pCommandBuffers = commandBuffers)
            {
                submitInfo = new VkSubmitInfo()
                {
                    waitSemaphoreCount = 0,
                    pWaitSemaphores = null,
                    pWaitDstStageMask = null,
                    commandBufferCount = (uint)commandBuffers.Length,
                    pCommandBuffers = pCommandBuffers,
                    signalSemaphoreCount = 0,
                    pSignalSemaphores = null,
                };
            }

            VkFence fence = renderFinishedFence.Handle;
            result = device.Api.vkResetFences(device.Handle, 1, &fence);
            result.AssertOk();
            result = device.Api.vkQueueSubmit(device.GraphicsQueue.Handle, 1, &submitInfo, fence);
            result.AssertOk();
            result = device.Api.vkWaitForFences(device.Handle, 1, &fence, true, 0xffffffff);
            result.AssertOk();

            VkMappedMemoryRange imageFlush = new VkMappedMemoryRange
            {
                memory = offscreenBuffer.Memory.Handle,
                offset = 0,
                size = VulkanConstants.WholeSize,
            };
            result = device.Api.vkInvalidateMappedMemoryRanges(device.Handle, 1, &imageFlush);
            result.AssertOk();
            memoryMapped = true;

        }


        private void PresentSharedTexture(CommandBuffer cmd, Fence renderFinishedFence)
        {
            VkResult result;

            VkSubmitInfo submitInfo;
            VkCommandBuffer[] commandBuffers =  { cmd.Handle };
            VkSemaphore[] waitSemaphores = { GlReadySemaphore };
            VkSemaphore[] signalSemaphores = { GlCompleteSemaphore };
            VkPipelineStageFlags[] waitFlags = { VkPipelineStageFlags.AllGraphics };
            fixed (VkCommandBuffer* pCommandBuffer = commandBuffers)
            fixed (VkSemaphore* pWaitSemaphores = waitSemaphores)
            fixed (VkSemaphore* pSignalSemaphores = signalSemaphores)
            fixed (VkPipelineStageFlags* pWaitFlags = waitFlags)
            {
                //if (UseSharedTexture)
                submitInfo = new VkSubmitInfo()
                {
                    waitSemaphoreCount = (uint)waitSemaphores.Length,
                    pWaitSemaphores = pWaitSemaphores,
                    pWaitDstStageMask = pWaitFlags,
                    
                    commandBufferCount = 1,
                    pCommandBuffers = pCommandBuffer,
                    signalSemaphoreCount = (uint)signalSemaphores.Length,
                    pSignalSemaphores = pSignalSemaphores,
                };

            }
            
            //cmd.End();
            
            result = device.Api.vkQueueSubmit(device.GraphicsQueue.Handle, 1, &submitInfo, VkFence.Null);
            result.AssertOk();
            device.GraphicsQueue.WaitIdle();
        }

        public OGlInteropRenderTarget(Device device, Int2 imageDimension, RenderPass renderPass, VkSampleCountFlags sampleCount, bool useSharedTexture)
        {
            this.renderPass = renderPass;
            this.device = device;
            this.imageDimension = imageDimension;
            this.useSharedTexture = useSharedTexture;
            this.sampleCountFlags = sampleCount;
            Build();
        }

        public override void Dispose()
        {
            if (memoryMapped)
            {
                device.Api.vkUnmapMemory(device.Handle, offscreenBuffer.Memory.Handle);
            }
        }

        private void Build()
        {
            // --------------------------
            // Image construction
            this.renderImage = new Image(device, imageDimension, VkFormat.R8g8b8a8Unorm, VkImageUsageFlags.ColorAttachment, VkMemoryPropertyFlags.DeviceLocal, sampleCountFlags);

            VkExternalMemoryHandleTypeFlags externalType;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                externalType = VkExternalMemoryHandleTypeFlags.OpaqueWin32KHR;
            }
            else
            {
                externalType = VkExternalMemoryHandleTypeFlags.OpaqueFdKHR;
            }
            this.renderImageView = new ImageView(device, renderImage, renderImage.Format, VkImageAspectFlags.Color);


            this.sharedImage = new Image(
            device,
            imageDimension,
            this.renderImage.Format,
            VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferSrc,
            VkMemoryPropertyFlags.DeviceLocal,
            VkSampleCountFlags._1,
            VkImageTiling.Optimal,
            externalType);
            this.sharedImageView = new ImageView(device, sharedImage, sharedImage.Format, VkImageAspectFlags.Color);

            // Use internal memory repsentation instead of calculating it directly by using dimensions + color
            // Alignment is important here 
            this.memoryImageShareSize = (UInt64)this.sharedImage.Memory.Size; 
            // --------------------------

            VkExternalSemaphoreHandleTypeFlags[] semaphoreTypeFlags = new VkExternalSemaphoreHandleTypeFlags[]
            {
                VkExternalSemaphoreHandleTypeFlags.OpaqueFdKHR,
                VkExternalSemaphoreHandleTypeFlags.OpaqueWin32KHR,
                VkExternalSemaphoreHandleTypeFlags.OpaqueWin32KmtKHR,
                VkExternalSemaphoreHandleTypeFlags.D3d12FenceKHR,
                VkExternalSemaphoreHandleTypeFlags.SyncFdKHR,
            };
            VkPhysicalDeviceExternalSemaphoreInfo externalSemaphoreInfo = new VkPhysicalDeviceExternalSemaphoreInfo();
            VkExternalSemaphoreProperties externalSemaphoreProperties = new VkExternalSemaphoreProperties();
            bool found = false;

            VkExternalSemaphoreHandleTypeFlags compatableSemaphoreType = VkExternalSemaphoreHandleTypeFlags.None;
            for (int i = 0; i < semaphoreTypeFlags.Length; i++)
            {
                externalSemaphoreInfo.handleType = semaphoreTypeFlags[i];
                device.Api.vkGetPhysicalDeviceExternalSemaphoreProperties(device.Physical.Handle, &externalSemaphoreInfo, &externalSemaphoreProperties);
                if (((externalSemaphoreProperties.compatibleHandleTypes & semaphoreTypeFlags[i]) != 0) &&
                    ((externalSemaphoreProperties.externalSemaphoreFeatures & VkExternalSemaphoreFeatureFlags.ExportableKHR) != 0))
                {
                    compatableSemaphoreType = semaphoreTypeFlags[i];
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new ArgumentException("VulkanDevice creation: can't find compatible semaphore type!");
            }
            // resolve extensions

            VkExportSemaphoreCreateInfo exportSemaphoreCreateInfo = new VkExportSemaphoreCreateInfo
            {
                handleTypes = compatableSemaphoreType,
            };

            VkSemaphoreCreateInfo semaphoreCreateInfo = new VkSemaphoreCreateInfo()
            {
                pNext = &exportSemaphoreCreateInfo,
            };
            VkResult result;

            if (useSharedTexture)
            {
                VkSemaphore newReadySemaphore;
                result = device.Api.vkCreateSemaphore(device.Handle, &semaphoreCreateInfo, null, &newReadySemaphore);
                result.AssertOk();
                glReadySemaphore = newReadySemaphore;

                VkSemaphore newCompleteSemaphore;
                result = device.Api.vkCreateSemaphore(device.Handle, &semaphoreCreateInfo, null, &newCompleteSemaphore);
                result.AssertOk();
                glCompleteSemaphore = newCompleteSemaphore;


                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    VkSemaphoreGetWin32HandleInfoKHR semaphoreReadyHandleInfo = new VkSemaphoreGetWin32HandleInfoKHR()
                    {
                        handleType = compatableSemaphoreType,
                        semaphore = glReadySemaphore,
                    };
                    HANDLE readyHandle;
                    result = device.Api.vkGetSemaphoreWin32HandleKHR(device.Handle, &semaphoreReadyHandleInfo, &readyHandle);
                    result.AssertOk();
                    this.glReadyHandle = readyHandle;


                    VkSemaphoreGetWin32HandleInfoKHR semaphoreCompleteHandleInfo = new VkSemaphoreGetWin32HandleInfoKHR()
                    {
                        handleType = compatableSemaphoreType,
                        semaphore = glCompleteSemaphore,
                    };
                    HANDLE completeHandle;
                    result = device.Api.vkGetSemaphoreWin32HandleKHR(device.Handle, &semaphoreCompleteHandleInfo, &completeHandle);
                    result.AssertOk();
                    this.glCompleteHandle = completeHandle;


                    VkMemoryGetWin32HandleInfoKHR memoryFdInfo = new VkMemoryGetWin32HandleInfoKHR
                    {
                        memory = this.sharedImage.Memory.Handle,
                        handleType = externalType,
                    };
                    HANDLE newMemoryHandle;
                    result = device.Api.vkGetMemoryWin32HandleKHR(device.Handle, &memoryFdInfo, &newMemoryHandle);
                    result.AssertOk();
                    this.sharedMemoryHandle = newMemoryHandle;
                }
                else
                {
                    VkSemaphoreGetFdInfoKHR semaphoreReadyHandleInfo = new VkSemaphoreGetFdInfoKHR()
                    {
                        handleType = compatableSemaphoreType,
                        semaphore = glReadySemaphore,
                    };
                    int readyHandle;
                    result = device.Api.vkGetSemaphoreFdKHR(device.Handle, &semaphoreReadyHandleInfo, &readyHandle);
                    result.AssertOk();
                    this.glReadyLinuxHandle = readyHandle;

                    VkSemaphoreGetFdInfoKHR semaphoreCompleteHandleInfo = new VkSemaphoreGetFdInfoKHR()
                    {
                        handleType = compatableSemaphoreType,
                        semaphore = glCompleteSemaphore,
                    };
                    int completeHandle;
                    result = device.Api.vkGetSemaphoreFdKHR(device.Handle, &semaphoreCompleteHandleInfo, &completeHandle);
                    result.AssertOk();
                    this.glCompleteLinuxHandle = completeHandle;

                    VkMemoryGetFdInfoKHR memoryFdInfo = new VkMemoryGetFdInfoKHR()
                    {
                        memory = this.sharedImage.Memory.Handle,
                        handleType = externalType,
                    };
                    int newLinuxMemoryHandle;
                    result = this.device.VulkanInstance.Api.vkGetMemoryFdKHR(device.Handle, &memoryFdInfo, &newLinuxMemoryHandle);
                    this.sharedLinuxMemoryHandle = newLinuxMemoryHandle;
                }
                result.AssertOk();

            }

            depthBuffer = new DepthBuffer(device, imageDimension, sampleCountFlags);
            depthBufferView = depthBuffer.CreateView();

            if (useSharedTexture == false)
            {
                offscreenBuffer = BitsOfNature.Rendering.Vulkan.LowLevel.Buffer.Create<byte>(device, (int)memoryImageShareSize, VkBufferUsageFlags.TransferDst, VkMemoryPropertyFlags.HostVisible);
            }


            frameBuffer = FrameBuffer.New(device, renderPass, imageDimension).
                AddAttachment(this.renderImageView.Handle).
                AddAttachment(depthBufferView.Handle).
                AddAttachment(this.sharedImageView.Handle).
                Build();
        }

        public override void Prepare()
        {
            if (memoryMapped)
            {
                device.Api.vkUnmapMemory(device.Handle, offscreenBuffer.Memory.Handle);
                memoryMapped = false;
            }
        }

        public override void Present(CommandBuffer cmd, Fence renderFinishedFence)
        {
            if (UseSharedTexture)
                PresentSharedTexture(cmd, renderFinishedFence);
            else
                PresentCopiedTexture(cmd, renderFinishedFence);
        }
    }
}
