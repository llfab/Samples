using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Platform.Interop;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using static Avalonia.OpenGL.GlConsts;

namespace CrossPlatformApp.Views
{
    public partial class OpenGlPage : UserControl
    {
        public OpenGlPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
