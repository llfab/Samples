// ===========================================================================
//                          B I T S   O F   N A T U R E
// ===========================================================================
//  This document contains proprietary information. It is the exclusive
//  confidential property of Stryker Corporation and its affiliates.
//
//  Copyright (c) 2025 Stryker
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaIosApp.Models;

namespace AvaloniaIosApp.ViewModels
{
    public partial class MainViewModel : ViewModel
    {
        #region Fields

        private readonly MainModel _mainModel;

        #endregion

        #region Bindable Fields

        private string _greetingsTitle = "Welcome to Avalonia on iOS!";

        public string GreetingTitle
        {
            get => _greetingsTitle;
            set => RaiseAndSetIfChanged(ref _greetingsTitle, value);
        }

        private string _greetingBits = "Powered by Bits of Nature!";

        public string GreetingBits
        {
            get => _greetingBits;
            set => RaiseAndSetIfChanged(ref _greetingBits, value);
        }

        #endregion

        #region Bindable Commands

        #endregion

        #region Constructor

        public MainViewModel()
        {
            TestRenderCommand = SimpleCommand.Create(TestRender3D, CanTestRender3D);
            AddCommand(TestRenderCommand);
            ToggleAnimatedRenderingCommand = SimpleCommand.Create(ToggleAnimatedRendering3D, CanToggleAnimatedRendering3D);
            AddCommand(ToggleAnimatedRenderingCommand);

            if (Design.IsDesignMode)
            {
            }
            else
            {
                _mainModel = AvaloniaIosAppSystem.Instance.MainModel;

                UpdateAll();
            }
        }

        public override void Dispose()
        {
        }

        #endregion

        #region Private Helpers

        private void UpdateAll()
        {
            if (OperatingSystem.IsWindows())
            {
                GreetingTitle = "Welcome to Avalonia on Windows!";
            }
            else if (OperatingSystem.IsLinux())
            {
                GreetingTitle = "Welcome to Avalonia on Linux!";
            }
            else if (OperatingSystem.IsMacOS())
            {
                GreetingTitle = "Welcome to Avalonia on MacOS!";
            }
            else if (OperatingSystem.IsIOS())
            {
                GreetingTitle = "Welcome to Avalonia on iOS!";
            }
            else if (OperatingSystem.IsAndroid())
            {
                GreetingTitle = "Welcome to Avalonia on Android!";
            }
            else
            {
                GreetingTitle = "Welcome to Avalonia!";
            }
        }

        #endregion

        #region Tab "3D"

        #region Private Instance Fields

        private SceneViewer3D _sceneViewer;

        private LowLevel.VulkanInstance _vulkanInstance;
        private LowLevel.Device _vulkanDevice;
        private RenderingContext _vulkanRenderingContext;

        public LowLevel.VulkanInstance Instance => _vulkanInstance;
        public LowLevel.Device Device => _vulkanDevice;
        public RenderingContext Context => _vulkanRenderingContext;

        #endregion

        #region Bindable Commands

        public SimpleCommand TestRenderCommand { get; private set; }
        public SimpleCommand ToggleAnimatedRenderingCommand { get; private set; }

        #endregion

        #region Init / Exit

        public void Init3D()
        {
            // set how to start
            bool enableVulkanLoader = false;

            // Enable vulkan layers only for real ios device because the binaries are not available for the simulator
            VulkanLibraryImportResolver.Configure(enableVulkanLoader);

            // Force static constructor in native AOT
            if (OperatingSystem.IsIOS())
            {
                VulkanApi.ForceInit();
            }

            _vulkanInstance = LowLevel.VulkanInstance.New()
                .EnableDebug(enableVulkanLoader, LowLevel.VulkanDebugCallback.Trace)
                //.SetByteCodeRepository(ByteCodeRepository.ForCompiling())
                //.SetByteCodeRepository(ByteCodeRepository.ForRecording("PrecompiledShaders"))
                //.SetByteCodeRepository(ByteCodeRepository.ForDirectory("PrecompiledShaders"))
                .SetByteCodeRepository(ByteCodeRepository.ForEmbeddedResources(typeof(MainViewModel).Assembly, "PrecompiledShaders"))
                .SetApplication("VulkanTests", new VersionInfo(0, 0, 0))
                .Build();

            string deviceSelectorName = "Default";
            LowLevel.DeviceSelector deviceSelector = deviceSelectorName switch
            {
                "Intel" => LowLevel.DeviceSelector.Intel,
                "Nvidia" => LowLevel.DeviceSelector.Nvidia,
                "Default" => LowLevel.DeviceSelector.Default,
                _ => throw new Exception($"Could not parse device selector[{deviceSelectorName}]!")
            };

            _vulkanDevice = LowLevel.Device.New(_vulkanInstance)
                .EnableGraphicsPipeline()
                .EnableOrderIndependentTransparency(IosUtils.IsIosDevice())
                .SetDeviceSelector(deviceSelector)
                .EnableDebugObjectTracking(true)
                .Build();

            _vulkanRenderingContext = new RenderingContext(_vulkanDevice, false);

        }

        public void Exit3D()
        {
            DisposeScene(_scene);
            _scene = null;

            CommonUtils.DisposeAndSetNull(ref _vulkanRenderingContext);
            IEnumerable<VulkanTrackedObject> unreleasedObjects = _vulkanDevice!.ObjectTracker.GetUnreleasedItems(x => x != Interop.VkObjectType.DeviceMemory);
            if (unreleasedObjects.Any())
            {
                _trace.Warning("Disposed rendering context => [{0}] resources associated with this device were not released!\n", unreleasedObjects.Count());
            }
            CommonUtils.DisposeAndSetNull(ref _vulkanDevice);
            CommonUtils.DisposeAndSetNull(ref _vulkanInstance);
        }

        #endregion

        #region Rendering

        public void SetSceneViewerControl(SceneViewer3D sceneViewer)
        {
            _sceneViewer = sceneViewer;
        }

        #endregion
    }
}
