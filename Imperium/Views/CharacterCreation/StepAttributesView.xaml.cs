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
    public partial class StepAttributesView : UserControl, IStepValidation, IStepDataCollector
    {
        private readonly CharacterCreationWindow _wizard;
        private const int MaxPoints = 27;
        private const int MaxPerStat = 15;
        private const double BarMaxWidth = 200;

        private readonly string[] _statNames = { "Strength", "Agility", "Endurance", "Intelligence", "Wisdom", "Charisma", "Luck" };
        private readonly string[] _statAbbrev = { "STR", "AGI", "END", "INT", "WIS", "CHA", "LCK" };
        private readonly string[] _statDescs =
        {
            "Physical power and carry capacity",
            "Speed, reflexes, and precision",
            "Stamina and toughness",
            "Magical power and learning",
            "Magic control and resistance",
            "Social presence and patron appeal",
            "Fortune and variance"
        };

        private int[] _values;
        private TextBlock[] _valueLabels = null!;
        private Rectangle[] _fillBars = null!;

        public StepAttributesView(CharacterCreationWindow wizard)
        {
            _wizard = wizard;
            InitializeComponent();

            // Restore or init
            _values = (int[])_wizard.Stats.Clone();

            BuildStatRows();
            UpdateDisplay();
        }

        public bool Validate()
        {
            int spent = _values.Sum();
            if (spent < MaxPoints)
            {
                var result = MessageBox.Show(
                    $"You still have {MaxPoints - spent} points to spend. Continue anyway?",
                    "Unspent Points", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }
            return true;
        }

        public void CollectData()
        {
            Array.Copy(_values, _wizard.Stats, 7);
            _wizard.UpdatePreviewPanel();
        }

        private void BuildStatRows()
        {
            _valueLabels = new TextBlock[7];
            _fillBars = new Rectangle[7];

            for (int i = 0; i < 7; i++)
            {
                int index = i; // capture for lambdas

                var row = new Grid { Margin = new Thickness(0, 0, 0, 10) };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });  // label
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });   // minus
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });   // value
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });   // plus
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // bar
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });  // description

                // Stat name
                var nameBlock = new TextBlock
                {
                    Text = $"{_statAbbrev[i]}  {_statNames[i]}",
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)FindResource("TextPrimaryBrush"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(nameBlock, 0);
                row.Children.Add(nameBlock);

                // Minus button
                var minusBtn = CreateStatButton("−", () =>
                {
                    if (_values[index] > 0) { _values[index]--; UpdateDisplay(); }
                });
                Grid.SetColumn(minusBtn, 1);
                row.Children.Add(minusBtn);

                // Value label
                var valueLabel = new TextBlock
                {
                    Text = "0",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = (Brush)FindResource("AccentBlueBrush"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                _valueLabels[i] = valueLabel;
                Grid.SetColumn(valueLabel, 2);
                row.Children.Add(valueLabel);

                // Plus button
                var plusBtn = CreateStatButton("+", () =>
                {
                    if (_values[index] < MaxPerStat && _values.Sum() < MaxPoints)
                    {
                        _values[index]++;
                        UpdateDisplay();
                    }
                });
                Grid.SetColumn(plusBtn, 3);
                row.Children.Add(plusBtn);

                // Progress bar background
                var barContainer = new Border
                {
                    Background = (Brush)FindResource("BgInputBrush"),
                    CornerRadius = new CornerRadius(4),
                    Height = 12,
                    Width = BarMaxWidth,
                    Margin = new Thickness(12, 0, 12, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var fillBar = new Rectangle
                {
                    Fill = (Brush)FindResource("AccentBlueBrush"),
                    RadiusX = 4,
                    RadiusY = 4,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 0
                };
                _fillBars[i] = fillBar;

                var barGrid = new Grid();
                barGrid.Children.Add(fillBar);
                barContainer.Child = barGrid;

                Grid.SetColumn(barContainer, 4);
                row.Children.Add(barContainer);

                // Description
                var descBlock = new TextBlock
                {
                    Text = _statDescs[i],
                    FontSize = 11,
                    Foreground = (Brush)FindResource("TextDimBrush"),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                Grid.SetColumn(descBlock, 5);
                row.Children.Add(descBlock);

                StatRowsPanel.Children.Add(row);
            }
        }

        private Button CreateStatButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Width = 32,
                Height = 32,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Content = text
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void UpdateDisplay()
        {
            int spent = _values.Sum();
            int remaining = MaxPoints - spent;
            PointsRemainingText.Text = remaining.ToString();

            // Color the remaining text based on state
            PointsRemainingText.Foreground = remaining == 0
                ? (Brush)FindResource("AccentBlueBrush")
                : (Brush)FindResource("AccentGoldBrush");

            for (int i = 0; i < 7; i++)
            {
                _valueLabels[i].Text = _values[i].ToString();

                // Update bar width (parent container width may not be available yet, so use fraction)
                double fraction = (double)_values[i] / MaxPerStat;
                // We'll use a fixed max width since the actual column width isn't easily measured here
                _fillBars[i].Width = fraction * 200;
            }

            // Also push live updates to the preview
            Array.Copy(_values, _wizard.Stats, 7);
            _wizard.UpdatePreviewPanel();
        }
    }
}
