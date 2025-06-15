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

namespace Imperium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GameSetupPage()); 
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoadGamePage());
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OptionsPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}