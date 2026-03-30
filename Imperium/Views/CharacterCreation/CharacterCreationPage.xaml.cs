using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using Imperium.Engine.Data.EntityModels.Character;
using Imperium.Data.Models;
using Imperium.Engine.Services;
using Imperium.Engine.Utilities;

namespace Imperium.Views.CharacterCreation
{
    /// <summary>
    /// Interaction logic for CharacterCreationPage.xaml
    /// </summary>
    public partial class CharacterCreationPage : Page
    {
        private bool _isLoaded = false;
        // Assuming these TextBox controls exist in the corresponding XAML file.  
        private TextBox FirstNameTextBox;
        private TextBox LastNameTextBox;
        private ComboBox RaceComboBox;
        private ComboBox BackgroundComboBox;
        private ComboBox ClassComboBox;
        private TextBox STRBoxValue;
        private TextBox AGIBoxValue;
        private TextBox ENDBoxValue;
        private TextBox INTBoxValue;
        private TextBox WISBoxValue;
        private TextBox CHABoxValue;
        private TextBox LCKBoxValue;
        //private TextBlock PointsRemainingText;
        private Button NextButton;
        public CharacterCreationPage()
        {
            InitializeComponent();
        }

        public CharacterCreationPage(string saveName, string? difficulty)
        {

            SaveName = saveName;
            Difficulty = difficulty;
            InitializeComponent();
            _isLoaded = true;
        }

        public string SaveName { get; }
        public string? Difficulty { get; }


        private void Customization_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClassSelectPage()); NavigationService?.Navigate(new ClassSelectPage());
            // TODO: Navigate to ClassSelectPage or open modal
        }

        private void Race_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Race selection clicked.");
        }

        private void Background_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Background selection clicked.");
        }

        private void Attributes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Attributes selection clicked.");
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            // Create a new Player object and populate its properties using an object initializer
            var player = new Player
            {
                FirstName = FirstNameTextBox?.Text.Trim() ?? string.Empty,
                LastName = LastNameTextBox?.Text.Trim() ?? string.Empty,
                //Pronouns = PronounsComboBox?.SelectedItem as string ?? string.Empty,
                RaceID = (RaceComboBox?.SelectedItem as Race)?.RaceID ?? 0,
                BackgroundID = (BackgroundComboBox?.SelectedItem as Background)?.BackgroundID ?? 0,
                //ClassID = (ClassComboBox?.SelectedItem as Class)?.ClassID ?? 0
            };

            // TODO: pull in any Attributes selections and add them to player.AttributesList

            // Now CurrentPlayer is populated and you can navigate on:
            //NavigationService?.Navigate(new CharacterCustomizationPage());
        }

        /// <summary>
        /// To be used later. 
        /// </summary>
        /*private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            string saveName = SaveNameTextBox.Text;

            // Get DB connection for this save file
            var connection = DbConnectionFactory.GetConnectionForSaveFile(saveName); // your implementation

            var repository = new SaveMetadataRepository(connection);
            var saveService = new SaveService(repository);

            saveService.SaveMetadata(saveName);

            MessageBox.Show("Save created!");
        }*/


        private int MaxPoints = 27;
       
        private void StatBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            int total = 0;
            int[] values = new int[7];
            TextBox[] boxes = { STRBox, AGIBox, ENDBox, INTBox, WISBox, CHABox, LCKBox };

            for (int i = 0; i < boxes.Length; i++)
            {
                var box = boxes[i];
                string input = box.Text;
                bool isValid = int.TryParse(input, out int val);

                if (!isValid) val = 0;

                // Clamp to [0, 15]
                if (val < 0) val = 0;
                if (val > 15) val = 15;

                // Check if adding this value would exceed the max allowed
                int tempTotal = total + val;

                if (tempTotal > MaxPoints)
                {
                    val = 0;
                }

                values[i] = val;
                total += val;

                // Sync sanitized value back to box if changed
                string valStr = val.ToString();
                if (box.Text != valStr)
                {
                    box.Text = valStr;
                    box.CaretIndex = box.Text.Length;
                }
            }

            int pointsRemaining = MaxPoints - total;
            PointsRemainingText.Text = $"Points Remaining: {pointsRemaining}";

            // Optionally enable "Next" only when fully allocated
            // NextButton.IsEnabled = (pointsRemaining == 0);
        }






    }
}
