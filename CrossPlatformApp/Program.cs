using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
//using Avalonia.ReactiveUI;
using Avalonia.Logging;
using System;

using BitsOfNature.Core.IO.Tracing;
using BitsOfNature.Core.IO.Serialization;
using CrossPlatformApp.Infrastructure;
using BitsOfNature.UI.Avalonia;
using System.Collections.Generic;
using Avalonia.OpenGL;

namespace CrossPlatformApp
{
    public class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                TraceApplication.Trace.Info("Main(): Starting App...");

                TraceApplication.Trace.Fatal("Main(): Test output");
                TraceApplication.Trace.Error("Main(): Test output");
                TraceApplication.Trace.Warning("Main(): Test output");
                TraceApplication.Trace.Info("Main(): Test output");
                TraceApplication.Trace.Debug("Main(): Test output");
                TraceApplication.Trace.Verbose("Main(): Test output");

                CrossPlatformAppSystem system = new CrossPlatformAppSystem();
                system.Init();

                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

                system.Exit();
            }
            catch (Exception ex)
            {
                TraceApplication.Trace.Error("Main(): Error\n{0}", ex);
            }

        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            AppBuilder app = AppBuilder.Configure<App>()
                .LogToBitsOfNatureTrace(TraceLevel.Default /* | TraceLevel.Info | TraceLevel.Debug | TraceLevel.Verbose*/)
                .UsePlatformDetect()
                .With(Environment.OSVersion.Platform == PlatformID.Win32NT ? new Win32PlatformOptions { UseWgl = false, WglProfiles = new List<GlVersion> { new GlVersion(GlProfileType.OpenGL, 4, 3) } } : null);
            //.LogToTrace(Avalonia.Logging.LogEventLevel.Warning)
            //.UseReactiveUI();
            return app;
        }
    }

    public static class Extensions
    {
        public static AppBuilder LogToBitsOfNatureTrace<AppBuilder>(this AppBuilder builder, TraceLevel level = TraceLevel.Default, params string[] areas)
        {
            Logger.Sink = new TracingLogSink(level, areas);
            return builder;
        }
    }
}
