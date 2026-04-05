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
    /// Interaction logic for StepRaceView.xaml
    /// </summary>
    public partial class StepRaceView : UserControl, IStepValidation, IStepDataCollector
    {
        private readonly CharacterCreationWindow _wizard;
        private string _selectedRace = "";

        // TODO: Replace with data from DB/entity models when races are built out
        private readonly List<RaceOption> _races = new()
        {
            new("Human",    "Versatile and ambitious",   "Adaptable people found in every kingdom. Jack of all trades.", "+1 to two stats of choice"),
            new("Elf",      "Ancient and graceful",      "Long-lived scholars and warriors with innate magical affinity.", "+2 INT, +1 AGI"),
            new("Dwarf",    "Stout and unyielding",      "Mountain-born smiths and fighters, renowned for endurance.", "+2 END, +1 STR"),
            new("Orc",      "Fierce and powerful",        "Warrior-born people with unmatched physical strength.", "+3 STR, -1 CHA"),
            new("Halfling", "Quick and lucky",            "Small but remarkably fortunate folk with nimble reflexes.", "+2 LCK, +1 AGI"),
            new("Beastkin", "Primal and adaptive",        "Shapeshifters with animal heritage granting unique abilities.", "+2 AGI, +1 END"),
        };

        public StepRaceView(CharacterCreationWindow wizard)
        {
            _wizard = wizard;
            InitializeComponent();
            RaceList.ItemsSource = _races;

            if (!string.IsNullOrEmpty(_wizard.SelectedRace))
            {
                _selectedRace = _wizard.SelectedRace;
                var match = _races.FirstOrDefault(r => r.Name == _selectedRace);
                if (match != null) ShowRaceDetails(match);
            }
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(_selectedRace))
            {
                MessageBox.Show("Please select a race.", "Race Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void CollectData()
        {
            _wizard.SelectedRace = _selectedRace;
            _wizard.UpdatePreviewPanel();
        }

        private void RaceCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string raceName)
            {
                _selectedRace = raceName;
                var race = _races.FirstOrDefault(r => r.Name == raceName);
                if (race != null) ShowRaceDetails(race);

                _wizard.SelectedRace = _selectedRace;
                _wizard.UpdatePreviewPanel();
            }
        }

        private void ShowRaceDetails(RaceOption race)
        {
            RaceDetailName.Text = race.Name;
            RaceDetailDesc.Text = race.Description;
            RaceDetailBonuses.Text = race.Bonuses;
        }

        private record RaceOption(string Name, string ShortDesc, string Description, string Bonuses);
    }
}
