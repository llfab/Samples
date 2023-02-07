using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AvaloniaAnimationTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (!_useRenderTransformCheckBox.IsChecked.Value)
            {
                _animGrid.Width = 600;
            }
            else
            {
                _animGrid.Classes.Add("large");
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_useRenderTransformCheckBox.IsChecked.Value)
            {
                _animGrid.Width = 200;
            }
            else
            {
                _animGrid.Classes.Remove("large");
            }
        }
    }
}
