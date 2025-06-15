using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Imperium.Views.CharacterCreation;

namespace Imperium.Views.MainMenu
{
    /// <summary>
    /// Interaction logic for GameSetupPage.xaml
    /// </summary>
    public partial class GameSetupPage : Page
    {
        public GameSetupPage()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            string saveName = SaveFileNameTextBox.Text;
            string difficulty = ((ComboBoxItem)DifficultyComboBox.SelectedItem).Content.ToString();

            if (string.IsNullOrWhiteSpace(saveName))
            {
                MessageBox.Show("Please enter a name for the save file.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Save to a profile manager or pass into game engine
            MessageBox.Show($"Starting game:\nSave: {saveName}\nDifficulty: {difficulty}", "Debug");


            // Example navigation or initialization
             NavigationService.Navigate(new CharacterCreationPage(saveName, difficulty));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
