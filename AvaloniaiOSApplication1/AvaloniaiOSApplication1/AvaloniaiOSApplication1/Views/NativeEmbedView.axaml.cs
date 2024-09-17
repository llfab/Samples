using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;

namespace AvaloniaiOSApplication1.Views
{
    public partial class NativeEmbedView : UserControl
    {
        public NativeEmbedView()
        {
            InitializeComponent();
        }
    }

    public class EmbeddedControl : NativeControlHost
    {
        public static INativeDemoControl Implementation { get; set; }

        static EmbeddedControl()
        {

        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            return Implementation?.CreateControl(parent, () => base.CreateNativeControlCore(parent))
                ?? base.CreateNativeControlCore(parent);
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            base.DestroyNativeControlCore(control);
        }
    }

    public interface INativeDemoControl
    {
        /// <param name="parent"></param>
        /// <param name="createDefault"></param>
        IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault);
    }
}