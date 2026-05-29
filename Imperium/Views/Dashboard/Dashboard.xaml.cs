using System.Windows;
using System.Windows.Controls;
using Imperium.Engine.Data.EntityModels.Character;
using Imperium.Engine.Data.EntityModels.Combat;
using Imperium.Engine.Data.Enums.Combat;
using Imperium.Engine.Services;
using Imperium.Engine.Utilities;
using Imperium.Views.Shell;

namespace Imperium.Views.Dashboard
{
    public partial class Dashboard : UserControl, IRefreshable
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        public void Refresh()
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            LoadPlayerOverview();
            LoadPlayerStats();
            LoadNextMatch();
        }

        private void LoadPlayerOverview()
        {
            var player = ApplicationUtilities.CurrentPlayer;
            if (player == null) return;

            string fullName = $"{player.FirstName} {player.LastName}".Trim();
            PlayerNameText.Text = string.IsNullOrEmpty(fullName) ? "Unknown Gladiator" : fullName;
            PlayerClassText.Text = "Gladiator";
            PlayerLevelText.Text = $"Level {player.Level}";
            PlayerRecordText.Text = "0-0 (0-0)";
            PlayerAgeText.Text = player.Age.ToString();
            PrestigeValueText.Text = "0";
        }

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

        private void LoadNextMatch()
        {
            var upcoming = GameManager.Calendar.GetUpcomingEvents(30);
            var nextMatch = upcoming
                .FirstOrDefault(e => e.EventType == Engine.Data.Enums.Calendar.CalendarEventType.ArenaMatch);

            if (nextMatch != null)
            {
                NextMatchTitle.Text = nextMatch.Title;
                NextMatchDetails.Text = $"Scheduled: {nextMatch.ScheduledDate}";
            }
            else
            {
                NextMatchTitle.Text = "No match scheduled";
                NextMatchDetails.Text = "Visit an arena to sign up.";
            }
        }

        // =================================================================
        //  DEV TOOLS — REMOVE BEFORE RELEASE
        // =================================================================

        private void TestArenaMatch_Click(object sender, RoutedEventArgs e)
        {
            var player = ApplicationUtilities.CurrentPlayer;
            if (player == null)
            {
                MessageBox.Show("No player loaded.", "Error");
                return;
            }

            var playerCombatant = Combatant.FromPlayer(player);
            playerCombatant.IsFanFavorite = true;

            var opponent = Combatant.CreateNPC("Iron Fang", player.Level, 7);

            var dice = new DiceService();
            var combatService = new CombatService(dice);
            var state = combatService.StartArenaMatch(
                playerCombatant, opponent,
                timeLimitSeconds: 600,
                arenaName: "The Ashen Coliseum",
                promoterName: "Marcus the Bold"
            );

            // TODO: Navigate to CombatView
            // For now, show the combat state as confirmation
            MessageBox.Show(
                $"Arena Match Started!\n\n" +
                $"{state.Player.Name} vs {state.Opponent.Name}\n" +
                $"Arena: {state.ArenaName}\n" +
                $"Promoter: {state.PromoterName}\n" +
                $"Time Limit: {state.TimeLimitSeconds / 60} minutes\n" +
                $"First Move: {state.ActiveFighter.Name}",
                "Test Arena Match",
                MessageBoxButton.OK);

            // TODO: Replace MessageBox with:
            // var combatView = new CombatView(combatService, state);
            // Navigate to combatView via the GameShellWindow content area
        }

        private void TestBattle_Click(object sender, RoutedEventArgs e)
        {
            var player = ApplicationUtilities.CurrentPlayer;
            if (player == null)
            {
                MessageBox.Show("No player loaded.", "Error");
                return;
            }

            var playerCombatant = Combatant.FromPlayer(player);
            var opponent = Combatant.CreateNPC("Roadside Bandit", Math.Max(1, player.Level - 1), 6);

            var dice = new DiceService();
            var combatService = new CombatService(dice);
            var state = combatService.StartBattle(playerCombatant, opponent);

            // TODO: Navigate to CombatView
            MessageBox.Show(
                $"Battle Started!\n\n" +
                $"{state.Player.Name} vs {state.Opponent.Name}\n" +
                $"Mode: {state.Mode}\n" +
                $"First Move: {state.ActiveFighter.Name}\n\n" +
                $"(Combat UI coming next)",
                "Test Battle",
                MessageBoxButton.OK);

            // TODO: Replace MessageBox with:
            // var combatView = new CombatView(combatService, state);
            // Navigate to combatView via the GameShellWindow content area
        }
    }
}
