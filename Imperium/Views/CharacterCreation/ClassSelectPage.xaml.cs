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
    /// Interaction logic for ClassSelectPage.xaml
    /// </summary>
    public partial class ClassSelectPage : Page
    {
        // test data supplier
        private Dictionary<string, List<string>> _itemsByTab;
        public ClassSelectPage()
        {
            InitializeComponent();
            BuildTestData();
            // populate initial tab
            PopulateForTab("Race");
        }

        private void BuildTestData()
        {
            _itemsByTab = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Race"] = new List<string> { "TestRace1", "TestRace2", "TestRace3" },
                ["Background"] = new List<string> { "TestBackground1", "TestBackground2", "TestBackground3" },
                ["Class"] = new List<string> { "TestClass1", "TestClass2", "TestClass3" },
                ["Attributes"] = new List<string> { "TestAttribute1", "TestAttribute2", "TestAttribute3" },
                ["Addictions"] = new List<string> { "TestAddiction1", "TestAddiction2", "TestAddiction3" }
            };
        }

        private void AttributeTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AttributeTabs.SelectedItem is TabItem tab)
            {
                PopulateForTab(tab.Header.ToString());
            }
        }

        private void PopulateForTab(string tabName)
        {
            // 1. ComboBox items
            if (_itemsByTab.TryGetValue(tabName, out var list))
                AvailableAttributesComboBox.ItemsSource = list;
            else
                AvailableAttributesComboBox.ItemsSource = null;

            // 2. Button text
            AddAttributeButton.Content = $"Add This {tabName.TrimEnd('s')}";

            // 3. Clear previous selection/description

            // 3. Select first entry so dropdown starts on it
            if (AvailableAttributesComboBox.Items.Count > 0)
                AvailableAttributesComboBox.SelectedIndex = 0;
        }


        private void AvailableAttributesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AvailableAttributesComboBox.SelectedItem is string name)
            {
                // Example description
                AttributeDescriptionTextBox.Text = $"Description Of {name}";
            }
            else
            {
                AttributeDescriptionTextBox.Clear();
            }
        }


    }
}
