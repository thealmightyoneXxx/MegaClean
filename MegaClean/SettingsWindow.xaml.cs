using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using MegaClean.Properties;


namespace MegaClean
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            if (File.Exists("user_settings.json"))
            {
                try
                {
                    string json = File.ReadAllText("user_settings.json");
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);

                    if (settings != null)
                    {
                        EnableDarkModeCheckBox.IsChecked = settings.EnableDarkMode;
                        ConfirmDeleteCheckBox.IsChecked = settings.ConfirmBeforeDelete;
                        AutoScanCheckBox.IsChecked = settings.AutoScanOnStartup;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Failed to load user settings:\n" + ex.Message);
                }
            }

            // Load other app settings from Properties.Settings
            DefaultExtensionsTextBox.Text = Properties.Settings.Default.DefaultExtensions;
            DefaultDaysTextBox.Text = Properties.Settings.Default.DefaultDays.ToString();
            AskBeforeDeleteCheckBox.IsChecked = Properties.Settings.Default.AskBeforeDelete;

        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Save the user settings
            var settings = new AppSettings
            {
                EnableDarkMode = EnableDarkModeCheckBox.IsChecked == true,
                ConfirmBeforeDelete = ConfirmDeleteCheckBox.IsChecked == true,
                AutoScanOnStartup = AutoScanCheckBox.IsChecked == true
            };
            Properties.Settings.Default.DefaultExtensions = DefaultExtensionsTextBox.Text;
            int.TryParse(DefaultDaysTextBox.Text, out int days);
            Properties.Settings.Default.DefaultDays = days;
            Properties.Settings.Default.AskBeforeDelete = AskBeforeDeleteCheckBox.IsChecked == true;
            Properties.Settings.Default.Save();
             string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText("user_settings.json", json);
            MessageBox.Show("Settings saved.");
            this.Close();
        }

        private void Grid_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
             

        }
    }
}
