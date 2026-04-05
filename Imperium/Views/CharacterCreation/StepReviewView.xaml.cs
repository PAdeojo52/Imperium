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
    /// Interaction logic for StepReviewView.xaml
    /// </summary>
    public partial class StepReviewView : UserControl
    {
        private readonly CharacterCreationWindow _wizard;

        private readonly string[] _statAbbrev = { "STR", "AGI", "END", "INT", "WIS", "CHA", "LCK" };

        public StepReviewView(CharacterCreationWindow wizard)
        {
            _wizard = wizard;
            InitializeComponent();
            PopulateReview();
        }

        private void PopulateReview()
        {
            // Identity
            string fullName = $"{_wizard.CharFirstName} {_wizard.CharLastName}".Trim();
            ReviewName.Text = string.IsNullOrEmpty(fullName) ? "Unknown" : fullName;
            ReviewRace.Text = string.IsNullOrEmpty(_wizard.SelectedRace) ? "None" : _wizard.SelectedRace;
            ReviewClass.Text = string.IsNullOrEmpty(_wizard.SelectedClass) ? "None" : _wizard.SelectedClass;
            ReviewPronouns.Text = _wizard.CharPronouns;

            // Stats
            StatsGrid.Children.Clear();
            for (int i = 0; i < 7; i++)
            {
                var panel = new StackPanel { Margin = new Thickness(0, 4, 0, 4) };

                var valueText = new TextBlock
                {
                    Text = _wizard.Stats[i].ToString(),
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Foreground = (Brush)FindResource("AccentBlueBrush"),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var labelText = new TextBlock
                {
                    Text = _statAbbrev[i],
                    FontSize = 11,
                    Foreground = (Brush)FindResource("TextMutedBrush"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                panel.Children.Add(valueText);
                panel.Children.Add(labelText);
                StatsGrid.Children.Add(panel);
            }

            // Game settings
            ReviewSaveName.Text = _wizard.SaveName;
            ReviewDifficulty.Text = _wizard.Difficulty;
        }
    }
}
