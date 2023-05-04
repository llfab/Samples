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

            _tcc.Content = "My first content";
        }

        private void OnCheckedTcc(object sender, RoutedEventArgs e)
        {
            _tcc.Content = "My first content";
        }

        private void OnUncheckedTcc(object sender, RoutedEventArgs e)
        {
            _tcc.Content = "My second much looooonger content";
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
