using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Vulkan;
using Avalonia.Vulkan.Skia;
using BitsOfNature.Core.Drawing.Interop;
using BitsOfNature.Core.IO.Serialization;
using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Interop.Skia;
using System;

namespace CrossPlatformAppVk
{
    internal class Program
    {

        private static void ScanSerializationAssemblies()
        {
            // To avoid exceptions from scanning non c# DLLs only selected DLLs are scanned

            // Scan main assembly
            SerializationTypeManager.Default.ScanBitsOfNatureCore();
        }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            TraceApplication.Trace.Fatal("Main(): Starting app...");
            TraceApplication.Trace.Error("Main(): Starting app...");
            TraceApplication.Trace.Warning("Main(): Starting app...");
            TraceApplication.Trace.Info("Main(): Starting app...");
            TraceApplication.Trace.Debug("Main(): Starting app...");
            TraceApplication.Trace.Verbose("Main(): Starting app...");

            GraphicsService.Configure(new SkiaGraphicsService());

            ScanSerializationAssemblies();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .With(new Win32PlatformOptions
                {
                    UseWgl = false,
                    AllowEglInitialization = false
                })
                .With(new VulkanOptions()
                {
                    UseDebug = true,
                    PreferDiscreteGpu = true
                })
                .UseVulkan()
                .UseSkia()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();



    }
}
