using System;
using System.Windows;
using VectorMaker.ControlsResources;
using VectorMaker.ViewModel;

namespace VectorMaker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e) //toDo (?)
        {
            base.OnClosed(e);
            App.Current.Shutdown();
        }

        private void MinimizeApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButtonWithIcon maximizeAppButton = sender as ToggleButtonWithIcon;
            if (maximizeAppButton.IsChecked == true)
            {
                WindowState = WindowState.Maximized;
                maximizeAppButton.IconKind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.WindowRestoreRegular;
            }
            else
            {
                WindowState = WindowState.Normal;
                maximizeAppButton.IconKind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.WindowMaximizeRegular;
            }
        }

        private void ExitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

