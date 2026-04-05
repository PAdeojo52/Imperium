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
    /// Interaction logic for StepClassView.xaml
    /// </summary>
    public partial class StepClassView : UserControl, IStepValidation, IStepDataCollector
    {
        private readonly CharacterCreationWindow _wizard;
        private string _selectedClass = "";

        // From design doc class list
        private readonly List<ClassOption> _classes = new()
        {
            new("Warrior",   "Melee combat specialist",   "Masters of physical combat, warriors dominate the arena with brute force and tactical prowess.", "Legionnaire, Gladiator, Juggernaut"),
            new("Wizard",    "Arcane spellcaster",         "Wielders of destructive and utility magic, feared in arenas that permit spellcasting.", "Battlemage, Summoner, Necromancer, Cryomancer"),
            new("Rogue",     "Speed and precision",        "Agile fighters who exploit openings, using finesse and deception to win.", "Assassin, Trickster, Duelist"),
            new("Alchemist", "Potions and debuffs",        "Chemical warfare specialists who brew mid-combat concoctions.", "Brewmaster, Plaguecrafter, Enchanter"),
            new("Mystic",    "Divine and mental powers",   "Channelers of divine energy and psychic force, versatile support-fighters.", "Oracle, Cleric, Mindbender"),
            new("Beastkin",  "Primal shapeshifter",        "Warriors bonded with animal spirits, shifting forms for combat advantage.", "Beastmaster, Shifter, Venator"),
        };

        public StepClassView(CharacterCreationWindow wizard)
        {
            _wizard = wizard;
            InitializeComponent();
            ClassList.ItemsSource = _classes;

            if (!string.IsNullOrEmpty(_wizard.SelectedClass))
            {
                _selectedClass = _wizard.SelectedClass;
                var match = _classes.FirstOrDefault(c => c.Name == _selectedClass);
                if (match != null) ShowClassDetails(match);
            }
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(_selectedClass))
            {
                MessageBox.Show("Please select a class.", "Class Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void CollectData()
        {
            _wizard.SelectedClass = _selectedClass;
            _wizard.UpdatePreviewPanel();
        }

        private void ClassCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string className)
            {
                _selectedClass = className;
                var cls = _classes.FirstOrDefault(c => c.Name == className);
                if (cls != null) ShowClassDetails(cls);

                _wizard.SelectedClass = _selectedClass;
                _wizard.UpdatePreviewPanel();
            }
        }

        private void ShowClassDetails(ClassOption cls)
        {
            ClassDetailName.Text = cls.Name;
            ClassDetailDesc.Text = cls.Description;
            ClassDetailSpecs.Text = cls.Specializations;
        }

        private record ClassOption(string Name, string ShortDesc, string Description, string Specializations);
    }
}
