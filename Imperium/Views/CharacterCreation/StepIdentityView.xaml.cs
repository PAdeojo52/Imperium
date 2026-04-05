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
    /// Interaction logic for StepIdentityView.xaml
    /// </summary>
    public partial class StepIdentityView : UserControl, IStepValidation, IStepDataCollector
    {
        private readonly CharacterCreationWindow _wizard;
        private string _selectedPronoun = "He/Him";
        private Border[] _pronounCards = null!;

        public StepIdentityView(CharacterCreationWindow wizard)
        {
            _wizard = wizard;
            InitializeComponent();
            _pronounCards = new[] { PronounHe, PronounShe, PronounThey, PronounOther };

            // Restore any previously entered data
            FirstNameBox.Text = _wizard.CharFirstName;
            LastNameBox.Text = _wizard.CharLastName;
            _selectedPronoun = _wizard.CharPronouns;
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Your gladiator needs a first name.", "Identity",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void CollectData()
        {
            _wizard.CharFirstName = FirstNameBox.Text.Trim();
            _wizard.CharLastName = LastNameBox.Text.Trim();
            _wizard.CharPronouns = _selectedPronoun;
            _wizard.UpdatePreviewPanel();
        }

        private void NameChanged(object sender, TextChangedEventArgs e)
        {
            _wizard.CharFirstName = FirstNameBox.Text.Trim();
            _wizard.CharLastName = LastNameBox.Text.Trim();
            _wizard.UpdatePreviewPanel();
        }

        private void SelectPronoun(string pronoun, Border selected)
        {
            _selectedPronoun = pronoun;
            var subtle = (Brush)FindResource("BorderSubtleBrush");
            var gold = (Brush)FindResource("AccentGoldBrush");
            foreach (var card in _pronounCards) card.BorderBrush = subtle;
            selected.BorderBrush = gold;
        }

        private void PronounHe_Click(object s, MouseButtonEventArgs e) => SelectPronoun("He/Him", PronounHe);
        private void PronounShe_Click(object s, MouseButtonEventArgs e) => SelectPronoun("She/Her", PronounShe);
        private void PronounThey_Click(object s, MouseButtonEventArgs e) => SelectPronoun("They/Them", PronounThey);
        private void PronounOther_Click(object s, MouseButtonEventArgs e) => SelectPronoun("Other", PronounOther);
    }
}
