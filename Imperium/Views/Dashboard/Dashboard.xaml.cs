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
using Imperium.Engine.Utilities;
using Imperium.Views.Shell;

namespace Imperium.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, IRefreshable
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadDashboardData();

        }
        /// <summary>
        /// Called by GameShellWindow after each turn advance.
        /// </summary>
        public void Refresh()
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            LoadPlayerOverview();
            LoadPlayerStats();
            LoadNextMatch();
            LoadPatronStatus();
            LoadInjuryStatus();
        }

        // =================================================================
        //  Player Overview (Row 1)
        // =================================================================

        private void LoadPlayerOverview()
        {
            var player = ApplicationUtilities.CurrentPlayer;
            if (player == null) return;

            // Name
            string fullName = $"{player.FirstName} {player.LastName}".Trim();
            PlayerNameText.Text = string.IsNullOrEmpty(fullName) ? "Unknown Gladiator" : fullName;

            // Class — TODO: look up class name from ClassID when class table is built
            PlayerClassText.Text = "Gladiator"; // Placeholder

            // Level
            PlayerLevelText.Text = $"Level {player.Level}";

            // Record — TODO: track wins/losses when combat system is built
            PlayerRecordText.Text = "0-0 (0-0)";

            // Age
            PlayerAgeText.Text = player.Age.ToString();

            // Prestige — TODO: pull from reputation system
            PrestigeValueText.Text = "0";

            // Location — TODO: pull from world map / player location
            PlayerLocationText.Text = "Starting City";
        }

        // =================================================================
        //  Core Stats (Row 2)
        // =================================================================

        private void LoadPlayerStats()
        {
            var player = ApplicationUtilities.CurrentPlayer;
            if (player == null) return;

            StatSTR.Text = player.Strength.ToString();
            StatAGI.Text = player.Agility.ToString();
            StatEND.Text = player.Endurance.ToString();
            StatINT.Text = player.Intelligence.ToString();
            StatWIS.Text = player.Wisdom.ToString();
            StatCHA.Text = player.Charisma.ToString();
            StatLCK.Text = player.Luck.ToString();
        }

        // =================================================================
        //  Next Match (Row 3 Right)
        // =================================================================

        private void LoadNextMatch()
        {
            // Check calendar for the next ArenaMatch event
            var upcoming = GameManager.Calendar
                .GetUpcomingEvents(30);

            var nextMatch = upcoming
                .FirstOrDefault(e => e.EventType == Engine.Data.Enums.Calendar.CalendarEventType.ArenaMatch);

            if (nextMatch != null)
            {
                NextMatchTitle.Text = nextMatch.Title;
                NextMatchDetails.Text = $"Scheduled: {nextMatch.ScheduledDate}";
                PlayMatchButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                NextMatchTitle.Text = "No match scheduled";
                NextMatchDetails.Text = "Visit an arena to sign up for a fight.";
                PlayMatchButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        // =================================================================
        //  Patron Status (Row 4 Left)
        // =================================================================

        private void LoadPatronStatus()
        {
            // TODO: Pull from patron/reputation system when built
            PatronStatusText.Text = "No patron yet. Win arena matches to attract attention.";
        }

        // =================================================================
        //  Injury Status (Row 4 Right)
        // =================================================================

        private void LoadInjuryStatus()
        {
            // TODO: Pull from player health/debuff system when built
            InjuryStatusText.Text = "No injuries. Ready to fight.";
        }
    }
}
