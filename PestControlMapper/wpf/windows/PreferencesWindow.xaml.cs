using PestControlMapper.shared.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PestControlMapper.wpf.windows
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        public ObservableCollection<GameConfiguration> CurrentConfigurations;

        public PreferencesWindow()
        {
            InitializeComponent();

            
            CurrentConfigurations = new ObservableCollection<GameConfiguration>();

            foreach(GameConfiguration configuration in PreferencesConfiguration.Preferences.GameConfigs)
            {
                CurrentConfigurations.Add(new GameConfiguration()
                {
                    ContentPath = (string)configuration.ContentPath.Clone(),
                    GameName = (string)configuration.GameName.Clone()
                });
            }

            ConfigSelectionBox.ItemsSource = CurrentConfigurations;

            SelectedConfigBox.ItemsSource = CurrentConfigurations;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Save currently selected config
            if (ConfigSelectionBox.Text != null && ConfigSelectionBox.Text != string.Empty && ConfigSelectionBox.SelectedItem == null)
            {
                CurrentConfigurations.Add(new GameConfiguration()
                {
                    ContentPath = ContentPathTextBox.Text,
                    GameName = ConfigSelectionBox.Text
                });
            }

            if (ConfigSelectionBox.Text != null && ConfigSelectionBox.Text != string.Empty && ConfigSelectionBox.SelectedItem != null)
            {
                (ConfigSelectionBox.SelectedItem as GameConfiguration).ContentPath = ContentPathTextBox.Text;
            }


            PreferencesConfiguration.Preferences.GameConfigs.Clear();


            foreach (GameConfiguration configuration in CurrentConfigurations)
            {
                PreferencesConfiguration.Preferences.GameConfigs.Add(new GameConfiguration()
                {
                    ContentPath = (string)configuration.ContentPath.Clone(),
                    GameName = (string)configuration.GameName.Clone()
                });
            }

            PreferencesConfiguration.SaveToFile(PreferencesConfiguration.PreferencesPath);
        }

        private void ConfigSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConfigSelectionBox.SelectedItem != null)
            {
                ContentPathTextBox.Text = (ConfigSelectionBox.SelectedItem as GameConfiguration).ContentPath;
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();
            
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ContentPathTextBox.Text = folderBrowser.SelectedPath;
            }
        }

        private void SelectedConfigBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedConfigBox.SelectedItem != null)
            {
                PreferencesConfiguration.Preferences.SelectedConfig = SelectedConfigBox.SelectedItem.ToString();
                PreferencesConfiguration.SaveToFile(PreferencesConfiguration.PreferencesPath);
            }
                
        }
    }
}
