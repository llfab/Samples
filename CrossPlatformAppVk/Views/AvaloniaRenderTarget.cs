using BitsOfNature.Core.Utils;
using BitsOfNature.Rendering.Vulkan.Engine;
using BitsOfNature.Rendering.Vulkan.Interop;
using BitsOfNature.Rendering.Vulkan.Interop.Win32;
using BitsOfNature.Rendering.Vulkan.LowLevel;
using BitsOfNature.Rendering.Vulkan.Memory;
using Avalonia.Vulkan;
using BitsOfNature.Core;


namespace CrossPlatformAppVk.Views
{
    public unsafe class AvaloniaRenderTarget : RenderTarget
    {
        private readonly RenderingContext _renderContext;
        public Device Device => _renderContext.Device;
        private Int2 _imageSize;
        private Image _sharedImage;
        private ImageView _sharedImageView;

        private HANDLE _sharedMemoryHandle;

        public override Int2 Size => _imageSize;
       
        public AvaloniaRenderTarget(RenderingContext renderContext, Device avaloniaDevice, Int2 imageSize, VulkanImage avaloniaImage, bool useAvaloniaImage) : base(renderContext, 96.0)
        {
            this._renderContext = renderContext;
            this._imageSize = imageSize;
            Image injectedAvaloniaImage =
                Image.Wrap(avaloniaDevice, new VkImage(avaloniaImage.Handle), VkFormat.B8G8R8A8UNorm, imageSize, VkSampleCountFlags._1);
            Build(injectedAvaloniaImage, avaloniaImage.MemoryHandle, useAvaloniaImage);
        }

        protected override void DisposeImpl()
        {
        }

        protected override FrameData BeginImpl()
        {
            return new FrameData(_sharedImageView, VkImageLayout.ColorAttachmentOptimal, null, null);
        }

        protected override void EndImpl()
        {
        }

        private void Build(Image image, ulong imageMemoryHandle, bool useAvaloniaImage)
        {
            Assert.That(image != null);
            if (useAvaloniaImage)
            {
                this._sharedImage = image;
                this._sharedImageView = new ImageView(Device, this._sharedImage, this._sharedImage.Format, VkImageAspectFlags.Color);
            }
            else
            {
                VkMemoryGetWin32HandleInfoKHR memoryFdInfo = new VkMemoryGetWin32HandleInfoKHR
                {
                    Memory = imageMemoryHandle,
                    HandleType = VkExternalMemoryHandleTypeFlags.OpaqueWin32KHR,
                };
                HANDLE newMemoryHandle;
                VkResult vkres = Device.Api.GetMemoryWin32HandleKHR(Device.Handle, &memoryFdInfo, &newMemoryHandle);
                vkres.AssertOk("GetMemoryWin32HandleKHR");
                this._sharedMemoryHandle = newMemoryHandle;

                VkExternalMemoryHandleTypeFlags externalType = VkExternalMemoryHandleTypeFlags.OpaqueWin32KHR;
                VkExternalMemoryImageCreateInfo externalMemoryImageCreateInfo = new VkExternalMemoryImageCreateInfo
                {
                    HandleTypes = VkExternalMemoryHandleTypeFlags.OpaqueWin32KHR,
                };
                VkImageCreateInfo imageInfo = new VkImageCreateInfo()
                {
                    pNext = &externalMemoryImageCreateInfo,
                    ImageType = VkImageType._2D,
                    Extent = new VkExtent3D(this._imageSize.Width, this._imageSize.Height, 1),
                    MipLevels = 1,
                    ArrayLayers = 1,
                    Format = VkFormat.B8G8R8A8UNorm,
                    Tiling = VkImageTiling.Optimal,
                    InitialLayout = VkImageLayout.Undefined,
                    Usage = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferSrc,
                    SharingMode = VkSharingMode.Exclusive,
                    Samples = VkSampleCountFlags._1,
                    Flags = 0
                };
                VkImage vImage;
                vkres = Device.Api.CreateImage(Device.Handle, &imageInfo, null, &vImage);
                vkres.AssertOk("CreateImage");
                this._sharedImage = Image.Wrap(Device, vImage, VkFormat.B8G8R8A8UNorm, this._imageSize, VkSampleCountFlags._1);
                VkImportMemoryWin32HandleInfoKHR importAllocInfo = new VkImportMemoryWin32HandleInfoKHR()
                {
                    HandleType = externalType,
                    Handle = this._sharedMemoryHandle,
                };
                VkMemoryRequirements requirements;
                Device.Api.GetImageMemoryRequirements(Device.Handle, this._sharedImage.Handle, &requirements);
                MemoryType memoryType = MemoryType.Find(Device, requirements, new[] { VkMemoryPropertyFlags.DeviceLocal });

                VkMemoryAllocateInfo info = new VkMemoryAllocateInfo()
                {
                    pNext = &importAllocInfo,
                    AllocationSize = (uint)requirements.Size,
                    MemoryTypeIndex = (uint)memoryType.Index,
                };
                VkDeviceMemory memoryHandle;
                vkres = Device.Api.AllocateMemory(Device.Handle, &info, null, &memoryHandle);
                vkres.AssertOk("AllocateMemory");

                vkres = Device.Api.BindImageMemory(Device.Handle, vImage, memoryHandle, 0);
                vkres.AssertOk("BindMemory");

                this._sharedImageView = new ImageView(Device, this._sharedImage, this._sharedImage.Format, VkImageAspectFlags.Color);

            }
        }

    }
}

