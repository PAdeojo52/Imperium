using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Imperium.Views.MainMenu;
using MahApps.Metro.Controls;

namespace Imperium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            var setup = new GameSetupWindow();
            setup.Show();
            this.Close();
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            var load = new LoadGamePage();
            load.Show();
            this.Close();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            var options = new OptionsPage();
            options.Owner = this;
            options.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}