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
using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.Enums.Calendar;
using Imperium.Engine.Utilities;
using MahApps.Metro.IconPacks;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Imperium.Views.Dashboard;


namespace Imperium.Views.Shell
{
    /// <summary>
    /// Interaction logic for GameShell.xaml
    /// </summary>
    public partial class GameShell : MetroWindow
    {
        private Button? _activeNavButton;

        public GameShell()
        {
            InitializeComponent();

            // Subscribe to calendar events from GameManager
            GameManager.OnDayAdvanced += OnDayAdvanced;

            // Set initial state
            UpdateDateDisplay();
            SetActiveNav(NavDashboard);
            MainContent.Content = new Dashboard.Dashboard();
        }


        // =================================================================
        //  Top Strip: Date & Season Display
        // =================================================================

        private void UpdateDateDisplay()
        {
            var date = GameManager.Calendar.CurrentDate;
            CurrentDateText.Text = date.ToString();

            // Update season icon and text
            var season = date.CurrentSeason();
            SeasonText.Text = season.ToString();
            SeasonIcon.Kind = season switch
            {
                Seasons.Winter => PackIconMaterialKind.Snowflake,
                Seasons.Spring => PackIconMaterialKind.Flower,
                Seasons.Summer => PackIconMaterialKind.WhiteBalanceSunny,
                Seasons.Autumn => PackIconMaterialKind.LeafMaple,
                _ => PackIconMaterialKind.CalendarMonth
            };
        }

        private void OnDayAdvanced(CalendarDate newDate)
        {
            // Ensure UI update happens on the UI thread
            Dispatcher.Invoke(() => UpdateDateDisplay());
        }

        // =================================================================
        //  Next Turn
        // =================================================================

        private void NextTurn_Click(object sender, RoutedEventArgs e)
        {
            GameManager.AdvanceTurn();

            // Refresh the current view if it implements IRefreshable
            if (MainContent.Content is IRefreshable refreshable)
            {
                refreshable.Refresh();
            }
        }

        // =================================================================
        //  Sidebar Navigation
        // =================================================================

        private void SetActiveNav(Button button)
        {
            // Reset previous
            if (_activeNavButton != null)
            {
                _activeNavButton.Style = (Style)FindResource("SidebarButtonStyle");
            }

            // Set new active
            button.Style = (Style)FindResource("SidebarButtonActiveStyle");
            _activeNavButton = button;
        }

        private void NavigateTo(Button navButton, UserControl view)
        {
            SetActiveNav(navButton);
            MainContent.Content = view;
        }

        private void NavDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(NavDashboard, new Dashboard.Dashboard());
        }

        private void NavCharacter_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNav(NavCharacter);
            // TODO: MainContent.Content = new CharacterView();
        }

        private void NavCalendar_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNav(NavCalendar);
            // TODO: MainContent.Content = new CalendarView();
        }

        private void NavArena_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNav(NavArena);
            // TODO: MainContent.Content = new ArenaView();
        }

        private void NavInventory_Click(object sender, RoutedEventArgs e)
        {
            SetActiveNav(NavInventory);
            // TODO: MainContent.Content = new InventoryView();
        }

        private void NavSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open settings flyout or navigate
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            GameManager.SaveGame();
            // TODO: Show save confirmation toast/notification
        }

        // =================================================================
        //  Cleanup
        // =================================================================

        protected override void OnClosed(EventArgs e)
        {
            GameManager.OnDayAdvanced -= OnDayAdvanced;
            base.OnClosed(e);
        }
    }

    /// <summary>
    /// Interface for views that need to refresh when the turn advances.
    /// </summary>
    public interface IRefreshable
    {
        void Refresh();
    }
}

