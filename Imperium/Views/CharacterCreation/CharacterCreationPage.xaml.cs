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

namespace Imperium.Views.CharacterCreation
{
    /// <summary>
    /// Interaction logic for CharacterCreationPage.xaml
    /// </summary>
    public partial class CharacterCreationPage : Page
    {
        private bool _isLoaded = false;
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


        private void Class_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Class selection clicked.");
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
            MessageBox.Show("Finish clicked.");
        }


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
