using PestControlEngine.GameManagers;
using PestControlEngine.Mapping;
using PestControlEngine.Mapping.Enums;
using PestControlEngine.Objects;
using PestControlMapper.Mapping;
using PestControlMapper.Objects;
using PestControlMapper.wpf.controls;
using PestControlMapper.wpf.structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PestControlMapper.wpf.windows
{
    /// <summary>
    /// Interaction logic for ObjectProperties.xaml
    /// </summary>
    public partial class ObjectProperties : Window
    {
        public Dictionary<string, GameObjectProperty> CurrentProperties { get; set; }
        public List<ComponentInfo> CurrentComponents { get; set; }

        public ObjectProperties()
        {
            InitializeComponent();

            CurrentProperties = new Dictionary<string, GameObjectProperty>();
            CurrentComponents = new List<ComponentInfo>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.mainWindow.ObjectComboBox != null)
            {
                MainComboBox.ItemsSource = MainWindow.mainWindow.ObjectComboBox.ItemsSource as ObservableCollection<ComboBoxObjectInfoItem>;
            }

            if (MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection != null && MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection.Count > 0)
            {
                var items = MainWindow.mainWindow.ObjectComboBox.ItemsSource as ObservableCollection<ComboBoxObjectInfoItem>;

                MainComboBox.IsEnabled = true;

                for(int i = 0; i < items.Count; i++)
                {
                    ComboBoxObjectInfoItem item = items.ElementAt(i);

                    var selectedObj = MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0] as MapGameObject;

                    if (selectedObj != null && item != null && item.GameObject != null && selectedObj != null && item.GameObject.ClassName == selectedObj.Info.ClassName)
                    {
                        MainComboBox.SelectedIndex = i;
                    }
                }
            }

            var compInfos = new ObservableCollection<ComboBoxComponentInfoItem>();

            ComponentComboBox.ItemsSource = compInfos;

            if (MainWindow.mainWindow.projectManager.objectInformation.ComponentInfos != null)
            {
                foreach (ComponentInfo componentInfo in MainWindow.mainWindow.projectManager.objectInformation.ComponentInfos)
                {
                    compInfos.Add(new ComboBoxComponentInfoItem()
                    {
                        RealName = componentInfo.RealName,
                        ComponentInfo = componentInfo
                    });
                }
            }
            

            UpdateProperties();
        }

        /// <summary>
        /// Updates properties in property viewer by adding them as controls after clearing the property viewer first.
        /// </summary>
        public void UpdateProperties()
        {
            PropertyStackPanel.Children.Clear();

            ObjectManager objectManager = MainWindowViewModel.viewModel.objectManager;

            if (objectManager.SelectionObject.Selection != null)
            {
                foreach(KeyValuePair<string, GameObjectProperty> propertyPair in CurrentProperties)
                {
                    CreatePropertyBox(propertyPair.Value, PropertyStackPanel);
                }

                foreach (ComponentInfo info in CurrentComponents)
                {
                    DropDownControl componentStrip = new DropDownControl();
                    componentStrip.HeaderText.Text = info.RealName;

                    componentStrip.Width = PropertyStackPanel.Width;

                    PropertyStackPanel.Children.Add(componentStrip);

                    foreach(KeyValuePair<string, ComponentProperty> pair in info.Properties)
                    {
                        CreatePropertyBox(pair.Value, componentStrip.ContentPanel);
                    }
                }
                Console.WriteLine(PropertyStackPanel.Children.Count);
                

            }
        }

        public void CreatePropertyBox(GameObjectProperty boundProperty, Panel ParentControl)
        {
            GameObjectProperty property = boundProperty;

            int propertyStripHeight = 20;

            int newPos = (ParentControl.Children.Count) * propertyStripHeight;

            PropertyViewer.Height += 20;
            PropertyViewer.Width = MainScrollViewer.ActualWidth;

            switch (property.propertyType)
            {
                case PropertyType.BOOL:
                    // Create property strip, set text name to property name and make events the property strip can hook to.
                    PropertyStripCheckbox propertyStripBool = new PropertyStripCheckbox();

                    propertyStripBool.PropertyHeader.Text = property.RealName;
                    propertyStripBool.PropertyCheckBox.IsChecked = property.GetAsBool();
                    propertyStripBool.Width = ParentControl.Width;

                    ParentControl.Children.Add(propertyStripBool);

                    Canvas.SetTop(propertyStripBool, newPos);

                    // Event to set property when the checkbox is checked or unchecked.
                    propertyStripBool.PropertyCheckBox.Click += (sender, e) =>
                    {
                        property.SetValue((bool)propertyStripBool.PropertyCheckBox.IsChecked);
                    };

                    break;
                case PropertyType.DOUBLE:
                    // Create property strip, set text name to property name and make events the property strip can hook to.
                    PropertyStripText propertyStripDouble = new PropertyStripText();

                    propertyStripDouble.PropertyHeader.Text = property.RealName;
                    propertyStripDouble.PropertyTextBox.Text = property.GetAsDouble().ToString();
                    propertyStripDouble.Width = MainScrollViewer.ActualWidth;


                    ParentControl.Children.Add(propertyStripDouble);

                    Canvas.SetTop(propertyStripDouble, newPos);

                    // Event to set property when the textbox is changed if the textbox is a valid double.
                    propertyStripDouble.PropertyTextBox.TextChanged += (sender, e) =>
                    {
                        double dummyOut;
                        if (double.TryParse(propertyStripDouble.PropertyTextBox.Text, out dummyOut))
                        {
                            propertyStripDouble.PropertyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(189, 195, 199));

                            property.SetValue(dummyOut);
                        }
                        else
                        {
                            propertyStripDouble.PropertyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(181, 74, 47));
                        }
                    };
                    break;
                case PropertyType.FLOAT:
                    // Create property strip, set text name to property name and make events the property strip can hook to.
                    PropertyStripText propertyStripFloat = new PropertyStripText();

                    propertyStripFloat.PropertyHeader.Text = property.RealName;
                    propertyStripFloat.PropertyTextBox.Text = property.GetAsFloat().ToString();
                    propertyStripFloat.Width = MainScrollViewer.ActualWidth;


                    ParentControl.Children.Add(propertyStripFloat);

                    Canvas.SetTop(propertyStripFloat, newPos);

                    // Event to set property when the textbox is changed if the textbox is a valid float.
                    propertyStripFloat.PropertyTextBox.TextChanged += (sender, e) =>
                    {
                        float dummyOut;
                        if (float.TryParse(propertyStripFloat.PropertyTextBox.Text, out dummyOut))
                        {
                            propertyStripFloat.PropertyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(189, 195, 199));

                            property.SetValue(dummyOut);
                        }
                        else
                        {
                            propertyStripFloat.PropertyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(181, 74, 47));
                        }
                    };
                    break;

                case PropertyType.INT:
                    // Create property strip, set text name to property name and make events the property strip can hook to.
                    PropertyStripText propertyStripInt = new PropertyStripText();

                    propertyStripInt.PropertyHeader.Text = property.RealName;
                    propertyStripInt.PropertyTextBox.Text = property.GetAsInt32().ToString();
                    propertyStripInt.Width = MainScrollViewer.ActualWidth;


                    ParentControl.Children.Add(propertyStripInt);

                    Canvas.SetTop(propertyStripInt, newPos);

                    // Event to set property when the textbox is changed if the textbox is a valid int.
                    propertyStripInt.PropertyTextBox.TextChanged += (sender, e) =>
                    {
                        int dummyOut;
                        if (int.TryParse(propertyStripInt.PropertyTextBox.Text, out dummyOut))
                        {
                            propertyStripInt.PropertyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(189, 195, 199));

                            property.SetValue(dummyOut);
                        }
                        else
                        {
                            propertyStripInt.PropertyTextBox.Foreground = new SolidColorBrush(Color.FromRgb(181, 74, 47));
                        }
                    };
                    break;

                case PropertyType.STRING:
                    // Create property strip, set text name to property name and make events the property strip can hook to.
                    PropertyStripText propertyStripString = new PropertyStripText();

                    propertyStripString.PropertyHeader.Text = property.RealName;
                    propertyStripString.PropertyTextBox.Text = property.GetAsString();
                    propertyStripString.Width = MainScrollViewer.ActualWidth;


                    ParentControl.Children.Add(propertyStripString);

                    Canvas.SetTop(propertyStripString, newPos);

                    // Event to set property when the textbox is changed if the textbox is a valid int.
                    propertyStripString.PropertyTextBox.TextChanged += (sender, e) =>
                    {
                        property.SetValue(propertyStripString.PropertyTextBox.Text);
                    };

                    break;
            }
        }

        private void MainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProperties();
        }

        // NOTE: make sure to call this method when making an instance of this window.
        public void PropertiesInit()
        {
            ObjectManager objectManager = MainWindowViewModel.viewModel.objectManager;

            if (objectManager.SelectionObject.Selection != null && objectManager.SelectionObject.Selection.Count > 0)
            {
                foreach (KeyValuePair<string, GameObjectProperty> propertyPair in objectManager.SelectionObject.Selection[0].Properties)
                {
                    CurrentProperties.Add(propertyPair.Key, new GameObjectProperty(propertyPair.Value.RealName, propertyPair.Value.propertyType)
                    {
                        DefaultValue = propertyPair.Value.DefaultValue,
                        CurrentValue = propertyPair.Value.CurrentValue
                    });
                }

                // Add components of selected object to dropdown list
                foreach(ComponentInfo component in objectManager.SelectionObject.Selection[0].Components)
                {
                    CurrentComponents.Add(component.Copy());
                }
            }

            UpdateProperties();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateProperties();
        }

        // Ok
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection != null && MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection.Count > 0)
            {
                MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0].Properties = CurrentProperties;

                MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0].Components = CurrentComponents;

                MainWindowViewModel.viewModel.UpdateObjectTree();

                Close();
            }
        }

        // Apply
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection != null && MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection.Count > 0)
            {
                MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0].Properties = CurrentProperties;

                MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0].Components = CurrentComponents;

                CurrentProperties = new Dictionary<string, GameObjectProperty>();

                CurrentComponents = new List<ComponentInfo>();

                MainWindowViewModel.viewModel.UpdateObjectTree();

                PropertiesInit();
            }
        }

        // Cancel
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Add currently selected component to current object
        private void AddComponentButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure we have atleast one valid thing selected
            if (MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection.Count > 0 && MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0] != null)
            {
                GameObject selection = MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection[0];

                // Add component info of currently selected combobox item to currently selected object.
                if (ComponentComboBox.SelectedItem != null && ComponentComboBox.SelectedItem is ComboBoxComponentInfoItem)
                {
                    ComboBoxComponentInfoItem item = ComponentComboBox.SelectedItem as ComboBoxComponentInfoItem;

                    CurrentComponents.Add(item.ComponentInfo.Copy());

                    UpdateProperties();
                }
            }
        }
    }
}
