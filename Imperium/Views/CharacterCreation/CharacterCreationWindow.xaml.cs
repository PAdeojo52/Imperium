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
using Imperium.Engine.Data.EntityModels.Character;
using Imperium.Engine.Utilities;
using Imperium.Views.Shell;
using MahApps.Metro.Controls;

namespace Imperium.Views.CharacterCreation
{
    /// <summary>
    /// Interaction logic for CharacterCreationWindow.xaml
    /// </summary>
    public partial class CharacterCreationWindow : MetroWindow
    {
        private int _currentStep = 1;
        private const int TotalSteps = 5;

        // Step data holders
        public string SaveName { get; }
        public string Difficulty { get; }

        // Character-in-progress
        public string CharFirstName { get; set; } = "";
        public string CharLastName { get; set; } = "";
        public string CharPronouns { get; set; } = "He/Him";
        public string SelectedRace { get; set; } = "";
        public string SelectedClass { get; set; } = "";
        public string SelectedBackground { get; set; } = "";
        public int[] Stats { get; set; } = new int[7]; // STR, AGI, END, INT, WIS, CHA, LCK

        // Step indicators
        private Border[] _stepCircles = null!;
        private TextBlock[] _stepLabels = null!;

        public CharacterCreationWindow(string saveName, string difficulty)
        {
            SaveName = saveName;
            Difficulty = difficulty;
            InitializeComponent();

            _stepCircles = new[] { Step1Circle, Step2Circle, Step3Circle, Step4Circle, Step5Circle };
            _stepLabels = new[] { Step1Label, Step2Label, Step3Label, Step4Label, Step5Label };

            ShowStep(1);
        }

        // =================================================================
        //  Step Navigation
        // =================================================================

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            // Validate current step before advancing
            if (!ValidateCurrentStep()) return;

            // Collect data from current step
            CollectStepData();

            if (_currentStep < TotalSteps)
            {
                _currentStep++;
                ShowStep(_currentStep);
            }
            else
            {
                // Final step — create character and launch game
                FinishCreation();
            }
        }

        private void BackStep_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep > 1)
            {
                _currentStep--;
                ShowStep(_currentStep);
            }
            else
            {
                // Go back to game setup
                var setup = new MainMenu.GameSetupWindow();
                setup.Show();
                this.Close();
            }
        }

        private void ShowStep(int step)
        {
            _currentStep = step;
            UpdateStepIndicators();

            // Swap content
            StepContent.Content = step switch
            {
                1 => new StepIdentityView(this),
                2 => new StepRaceView(this),
                3 => new StepClassView(this),
                4 => new StepAttributesView(this),
                5 => new StepReviewView(this),
                _ => null
            };

            // Update button labels
            BackStepButton.Content = step == 1 ? "Cancel" : "Back";
            NextStepButton.Content = step == TotalSteps ? "Create Gladiator" : "Next";

            UpdatePreviewPanel();
        }

        private void UpdateStepIndicators()
        {
            for (int i = 0; i < TotalSteps; i++)
            {
                int stepNum = i + 1;
                if (stepNum < _currentStep)
                {
                    _stepCircles[i].Style = (Style)FindResource("StepCompletedStyle");
                    _stepLabels[i].Foreground = (Brush)FindResource("TextSecondaryBrush");
                }
                else if (stepNum == _currentStep)
                {
                    _stepCircles[i].Style = (Style)FindResource("StepActiveStyle");
                    _stepLabels[i].Foreground = (Brush)FindResource("AccentGoldBrush");
                }
                else
                {
                    _stepCircles[i].Style = (Style)FindResource("StepInactiveStyle");
                    _stepLabels[i].Foreground = (Brush)FindResource("TextMutedBrush");
                }
            }
        }

        // =================================================================
        //  Preview Panel
        // =================================================================

        public void UpdatePreviewPanel()
        {
            string fullName = $"{CharFirstName} {CharLastName}".Trim();
            PreviewName.Text = string.IsNullOrEmpty(fullName) ? "Your Gladiator" : fullName;

            string subtitle = "";
            if (!string.IsNullOrEmpty(SelectedRace)) subtitle += SelectedRace;
            if (!string.IsNullOrEmpty(SelectedClass)) subtitle += (subtitle.Length > 0 ? " • " : "") + SelectedClass;
            PreviewSubtitle.Text = string.IsNullOrEmpty(subtitle) ? "Awaiting creation..." : subtitle;

            SummaryRace.Text = string.IsNullOrEmpty(SelectedRace) ? "—" : SelectedRace;
            SummaryClass.Text = string.IsNullOrEmpty(SelectedClass) ? "—" : SelectedClass;
            SummaryBackground.Text = string.IsNullOrEmpty(SelectedBackground) ? "—" : SelectedBackground;

            string[] labels = { "STR", "AGI", "END", "INT", "WIS", "CHA", "LCK" };
            TextBlock[] previews = { PreviewSTR, PreviewAGI, PreviewEND, PreviewINT, PreviewWIS, PreviewCHA, PreviewLCK };
            for (int i = 0; i < 7; i++)
            {
                previews[i].Text = $"{labels[i]}: {(Stats[i] > 0 ? Stats[i].ToString() : "—")}";
            }
        }

        // =================================================================
        //  Validation & Data Collection
        // =================================================================

        private bool ValidateCurrentStep()
        {
            if (StepContent.Content is IStepValidation validator)
            {
                return validator.Validate();
            }
            return true;
        }

        private void CollectStepData()
        {
            if (StepContent.Content is IStepDataCollector collector)
            {
                collector.CollectData();
            }
        }

        // =================================================================
        //  Finish
        // =================================================================

        private void FinishCreation()
        {
            // Build the player entity
            var player = new Player
            {
                FirstName = CharFirstName,
                LastName = CharLastName,
                Level = 1,
                XP = 0,
                Age = 18,
                HP = 100,
                MP = 50,
                Strength = Stats[0],
                Agility = Stats[1],
                Endurance = Stats[2],
                Intelligence = Stats[3],
                Wisdom = Stats[4],
                Charisma = Stats[5],
                Luck = Stats[6]
            };

            // Set as current player
            ApplicationUtilities.CurrentPlayer = player;

            // Initialize the game engine
            GameManager.InitializeNewGame(1); // TODO: get real save ID

            // Open the game shell
            var shell = new GameShell();
            shell.Show();
            this.Close();
        }
    }

    // =================================================================
    //  Step Interfaces
    // =================================================================

    /// <summary>Implement on step UserControls that need validation before advancing.</summary>
    public interface IStepValidation
    {
        bool Validate();
    }

    /// <summary>Implement on step UserControls that need to push data back to the wizard.</summary>
    public interface IStepDataCollector
    {
        void CollectData();
    }
}
