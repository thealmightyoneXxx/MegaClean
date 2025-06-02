using System;
using System.IO;
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
using System.Text.Json;
using WinForms = System.Windows.Forms;


namespace MegaClean
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplySettings();

            if (File.Exists("user_settings.json"))
            {
                var json = File.ReadAllText("user_settings.json");
                var settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (settings?.AutoScanOnStartup == true)
                {
                    Scan_Click(this, new RoutedEventArgs());
                }
            }

            // Apply default text from app settings
            ExtensionsTextBox.Text = Properties.Settings.Default.DefaultExtensions;
            DaysTextBox.Text = Properties.Settings.Default.DefaultDays.ToString();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }


        private void FilePreviewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Im looking into users being able to remove added items from the 
        }
        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new WinForms.FolderBrowserDialog())
            {
                WinForms.DialogResult result = dialog.ShowDialog();

                if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedFolder = dialog.SelectedPath;

                    if (!FolderListBox.Items.Contains(selectedFolder))
                    {
                        FolderListBox.Items.Add(selectedFolder);
                    }
                }
            }
        }
        //  SCAN BUTTON (We will expand on this more ) 
        private void Scan_Click(object sender, RoutedEventArgs e)
        {
           
            var extensions = ExtensionsTextBox.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim().ToLower())
                .ToList();

            if (!int.TryParse(DaysTextBox.Text, out int daysOld))
            {
                System.Windows.MessageBox.Show("Please enter a valid number for days.");
                return;
            }

            DateTime thresholdDate = DateTime.Now.AddDays(-daysOld);

            foreach (string folder in FolderListBox.Items)
            {
                if (!Directory.Exists(folder))
                    continue;

                try
                {
                    var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        string ext = System.IO.Path.GetExtension(file)?.ToLower();
                        DateTime lastAccess = File.GetLastAccessTime(file);

                        // Check file against filters
                        if (extensions.Contains(ext) &&
                            lastAccess < thresholdDate &&
                            !File.Exists(file + ".priority")) 
                        {
                            FilePreviewList.Items.Add(file); // Show in preview list
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error scanning folder:\n{folder}\n{ex.Message}");
                }
            }

            System.Windows.MessageBox.Show($"Scan complete. Found {FilePreviewList.Items.Count} file(s).");

       
        }  

        private void ClearFolders_Click(object sender, RoutedEventArgs e)
             {   
             FolderListBox.Items.Clear();
             }


        // CLEAN BUTTON (placeholder )
        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.AskBeforeDelete)
            {   //Confirmation of Deletion 
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to delete {FilePreviewList.Items.Count} files?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return; // User cancelled
            }
            int deletedCount = 0;
            List<string> failedFiles = new List<string>();
                foreach (var item in FilePreviewList.Items)
            {
                try
                {
                    string filePath = item.ToString();
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        deletedCount++;
                    }
                }
                catch (Exception Errx)
                {
                    failedFiles.Add(item.ToString());
                }
            }
            System.Windows.MessageBox.Show(
             $"Deleted {deletedCount} file(s).\nFailed: {failedFiles.Count}",
             "Cleanup Complete",
             MessageBoxButton.OK,
             MessageBoxImage.Information);
            FilePreviewList.Items.Clear();
        }

        private void FilePreviewList_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
     
        private void Settings_Click(object sender, RoutedEventArgs e)
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.Owner = this;
                settingsWindow.ShowDialog();
            //Update Settings when close
            ApplySettings();
            }

        private void ApplySettings()
        {
            if (File.Exists("user_settings.json"))
            {
                try
                {
                    string json = File.ReadAllText("user_settings.json");
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);

                    if (settings != null)
                    {
                        // Apply Dark Mode if enabled
                        if (settings.EnableDarkMode)
                        {
                            this.Background = new SolidColorBrush(Colors.DarkSlateGray);
                            foreach (var control in FindVisualChildren<System.Windows.Controls.TextBox>(this))
                                control.Background = new SolidColorBrush(Colors.LightGray);
                        }
                        else
                        {
                            this.Background = new SolidColorBrush(Colors.White);
                            foreach (var control in FindVisualChildren<System.Windows.Controls.TextBox>(this))
                                control.Background = new SolidColorBrush(Colors.White);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Failed to reload settings:\n" + ex.Message);
                }
            }

           
            ExtensionsTextBox.Text = Properties.Settings.Default.DefaultExtensions;
            DaysTextBox.Text = Properties.Settings.Default.DefaultDays.ToString();
        }


    }
}
