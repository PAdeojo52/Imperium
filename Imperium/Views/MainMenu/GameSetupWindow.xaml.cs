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
using System.Windows.Shapes;
using Imperium.Views.CharacterCreation;
using MahApps.Metro.Controls;

namespace Imperium.Views.MainMenu
{
    /// <summary>
    /// Interaction logic for GameSetupWindow.xaml
    /// </summary>
    public partial class GameSetupWindow : MetroWindow
    {
        private string _selectedDifficulty = "Medium";

        public GameSetupWindow()
        {
            InitializeComponent();
        }

        // --- Difficulty Card Selection ---

        private void ResetDifficultyCards()
        {
            DiffEasy.BorderBrush = (System.Windows.Media.Brush)FindResource("BorderSubtleBrush");
            DiffMedium.BorderBrush = (System.Windows.Media.Brush)FindResource("BorderSubtleBrush");
            DiffHard.BorderBrush = (System.Windows.Media.Brush)FindResource("BorderSubtleBrush");
        }

        private void DiffEasy_Click(object sender, MouseButtonEventArgs e)
        {
            ResetDifficultyCards();
            DiffEasy.BorderBrush = (System.Windows.Media.Brush)FindResource("AccentBlueBrush");
            _selectedDifficulty = "Easy";
        }

        private void DiffMedium_Click(object sender, MouseButtonEventArgs e)
        {
            ResetDifficultyCards();
            DiffMedium.BorderBrush = (System.Windows.Media.Brush)FindResource("AccentGoldBrush");
            _selectedDifficulty = "Medium";
        }

        private void DiffHard_Click(object sender, MouseButtonEventArgs e)
        {
            ResetDifficultyCards();
            DiffHard.BorderBrush = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E53935"));
            _selectedDifficulty = "Hard";
        }

        // --- Navigation ---

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            string saveName = SaveFileNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(saveName))
            {
                MessageBox.Show("Please enter a name for your save file.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var creation = new CharacterCreationWindow(saveName, _selectedDifficulty);
            creation.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
