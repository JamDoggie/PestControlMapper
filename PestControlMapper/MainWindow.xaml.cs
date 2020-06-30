using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using PestControlEngine.Mapping;
using PestControlEngine.Objects;
using PestControlMapper.Mapping.map_extraction;
using PestControlMapper.Objects;
using PestControlMapper.shared.Managers;
using PestControlMapper.wpf.structs;
using PestControlMapper.wpf.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace PestControlMapper
{
    public partial class MainWindow : Window
    {
        public ProjectManager projectManager = new ProjectManager();

        public static MainWindow mainWindow;

        public bool gridDown = false;

        public MainWindow()
        {
            InitializeComponent();

            mainWindow = this;

            if (!File.Exists(PreferencesConfiguration.PreferencesPath))
            {
                PreferencesConfiguration.SaveToFile(PreferencesConfiguration.PreferencesPath);
            }
            else
            {
                PreferencesConfiguration.LoadFromFile(PreferencesConfiguration.PreferencesPath);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow preferencesWindow = new PreferencesWindow();
            preferencesWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            projectManager.LoadObjectInfos("ratcontrol.pcoi");
            UpdateObjectBox();

            GridSizeTextBox.Text = MainWindowViewModel.viewModel.GridSize.ToString();
            GridCheckBox.IsChecked = MainWindowViewModel.viewModel.GridVisible;

            Color backColor = MainWindowViewModel.viewModel.BackgroundColor;

            BackColorTextBox.Text = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(255, backColor.R, backColor.G, backColor.B));

            UpdateTitleBar();
        }

        public void UpdateTitleBar()
        {
            if (string.IsNullOrEmpty(projectManager.ProjectPath))
            {
                Title = "Pest Control Mapper";
            }
            else
            {
                FileInfo info = new FileInfo(projectManager.ProjectPath);
                Title = $"Pest Control Mapper - {info.Name}";
            }
        }

        public void UpdateObjectBox()
        {
            ObjectComboBox.Items.Clear();

            var objectInfos = new ObservableCollection<ComboBoxObjectInfoItem>();

            ObjectComboBox.ItemsSource = objectInfos;
            
            foreach(GameObjectInfo info in projectManager.objectInformation.ObjectInfos)
            {
                objectInfos.Add(new ComboBoxObjectInfoItem()
                {
                    GameObject = info,
                    RealName = info.DisplayName
                });
            }
        }

        Vector2 previousMousePos = Vector2.Zero;
        bool previousMouseInit = false;

        private void MonoGameContentControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (previousMouseInit != false)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    GameCamera camera = MainWindowViewModel.viewModel.objectManager.CurrentCamera;

                    Vector2 worldNew = Vector2.Transform(new Vector2((float)e.GetPosition(Viewport).X, (float)e.GetPosition(Viewport).Y), Matrix.Invert(camera.Transform));
                    Vector2 worldOld = Vector2.Transform(new Vector2(previousMousePos.X, previousMousePos.Y), Matrix.Invert(camera.Transform));
                    camera.SetPosition(camera.GetPosition() + (worldNew - worldOld));
                }
            }
            previousMouseInit = true;
            previousMousePos = new Vector2((float)e.GetPosition(Viewport).X, (float)e.GetPosition(Viewport).Y);

            if (MainWindowViewModel.viewModel.guiManager.GetScreen(MainWindowViewModel.viewModel.guiManager.CurrentScreen) != null)
            {
                foreach (PestControlEngine.GUI.UIElement element in MainWindowViewModel.viewModel.guiManager.GetScreen(MainWindowViewModel.viewModel.guiManager.CurrentScreen).GetControls())
                {
                    element.WPFMouseMove(e);
                }
            }

            MainWindowViewModel.viewModel.WPFMouseMove(e);
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                MainWindowViewModel.viewModel.objectManager.CurrentCamera.Zoom += 0.25f;
            if (e.Delta < 0)
                MainWindowViewModel.viewModel.objectManager.CurrentCamera.Zoom -= 0.25f;
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindowViewModel.viewModel.guiManager.GetScreen(MainWindowViewModel.viewModel.guiManager.CurrentScreen) != null)
            {
                foreach (PestControlEngine.GUI.UIElement element in MainWindowViewModel.viewModel.guiManager.GetScreen(MainWindowViewModel.viewModel.guiManager.CurrentScreen).GetControls())
                {
                    element.WPFMouseDown(e);
                }
            }

            Viewport.Focus();

            MainWindowViewModel.viewModel.WPFMouseDown(e);
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MainWindowViewModel.viewModel.guiManager.GetScreen(MainWindowViewModel.viewModel.guiManager.CurrentScreen) != null)
            {
                foreach (PestControlEngine.GUI.UIElement element in MainWindowViewModel.viewModel.guiManager.GetScreen(MainWindowViewModel.viewModel.guiManager.CurrentScreen).GetControls())
                {
                    element.WPFMouseUp(e);
                }
            }

            MainWindowViewModel.viewModel.WPFMouseUp(e);
        }

        private void ObjectComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MainWindowViewModel.viewModel.UpdateSelection();
        }

        // Properties (Context Menu)
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ObjectProperties propertiesWindow = new ObjectProperties();

            propertiesWindow.PropertiesInit();

            propertiesWindow.ShowDialog();
        }

        // Delete (Context Menu)
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection != null && MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection.Count > 0)
            {
                foreach(GameObject obj in MainWindowViewModel.viewModel.objectManager.SelectionObject.Selection)
                {
                    MainWindowViewModel.viewModel.objectManager.GetObjects().Remove(obj);
                }

                MainWindowViewModel.viewModel.UpdateObjectTree();
            }
        }

        public void SaveProject(bool forceDialog = false)
        {
            Mouse.OverrideCursor = Cursors.AppStarting;
            if (projectManager.ProjectPath == string.Empty || forceDialog)
            {
                SaveFileDialog projectDialog = new SaveFileDialog();
                projectDialog.Filter = "Pest Control Map Project (*.mapproj)|*.mapproj";

                // (== true instead of just leaving it because ShowDialog() returns a nullable bool instead of a regular bool)
                if (projectDialog.ShowDialog() == true)
                {
                    var stream = File.Create(projectDialog.FileName);

                    projectManager.ProjectPath = projectDialog.FileName;

                    // Always make sure to dispose your streams, kids.
                    stream.Dispose();
                }
                else
                {
                    // If the user cancels the save file dialog, cancel saving entirely.
                    Mouse.OverrideCursor = null;
                    return;
                }
            }

            // We now are certain the project file exists if we've gotten this far in the scope, write to it.
            MapJSON map = GetCurrentMap();

            string jsonOutput = JsonConvert.SerializeObject(map, Formatting.Indented);

            File.WriteAllText(projectManager.ProjectPath, jsonOutput);

            Mouse.OverrideCursor = null;
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }

        private void MenuItem_Saveas(object sender, RoutedEventArgs e)
        {
            SaveProject(true);
        }

        public void OpenProject()
        {
            OpenFileDialog projectDialog = new OpenFileDialog();
            projectDialog.Filter = "Pest Control Map Project (*.mapproj)|*.mapproj";

            if (projectDialog.ShowDialog() == true)
            {
                projectManager.ProjectPath = projectDialog.FileName;

                // Clear objects of map objects
                List<MapGameObject> toRemove = new List<MapGameObject>();

                foreach(GameObject obj in MainWindowViewModel.viewModel.objectManager.GetObjects())
                {
                    MapGameObject mapObject = obj as MapGameObject;

                    if (mapObject != null)
                    {
                        toRemove.Add(mapObject);
                    }
                }

                foreach(MapGameObject obj in toRemove)
                {
                    MainWindowViewModel.viewModel.objectManager.GetObjects().Remove(obj);
                }

                // Add new map objects from json file
                string jsonInput = File.ReadAllText(projectDialog.FileName);

                MapJSON map = JsonConvert.DeserializeObject<MapJSON>(jsonInput);

                MainWindowViewModel.viewModel.BackgroundColor = new Color(map.BackgroundR, map.BackgroundG, map.BackgroundB);

                foreach (MapObjectJSON objectJson in map.Objects)
                {
                    MapGameObject mapObject = new MapGameObject(objectJson.Info)
                    {
                        Properties = objectJson.Properties,
                        Layer = objectJson.Layer,
                        Name = objectJson.Name,
                        HelperRectangles = objectJson.Info.HelperRectangles,
                        Components = objectJson.Components
                    };

                    mapObject.SetPosition(new Vector2(objectJson.PositionX, objectJson.PositionY));

                    mapObject.CurrentAnimation = objectJson.Info.DefaultAnimation;

                    MainWindowViewModel.viewModel.objectManager.GetObjects().Add(mapObject);
                }
            }
            else
            {
                return;
            }

            // Update background color box
            Color backColor = MainWindowViewModel.viewModel.BackgroundColor;

            BackColorTextBox.Text = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(255, backColor.R, backColor.G, backColor.B));

            UpdateTitleBar();
        }

        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            OpenProject();

            MainWindowViewModel.viewModel.UpdateObjectTree();
        }

        public MapJSON GetCurrentMap()
        {
            Color bgColor = MainWindowViewModel.viewModel.BackgroundColor;

            MapJSON map = new MapJSON()
            {
                BackgroundR = bgColor.R,
                BackgroundG = bgColor.G,
                BackgroundB = bgColor.B
            };

            foreach (GameObject obj in MainWindowViewModel.viewModel.objectManager.GetObjects())
            {
                MapGameObject mapObject = obj as MapGameObject;

                if (mapObject != null)
                {
                    map.Objects.Add(new MapObjectJSON()
                    {
                        Layer = mapObject.Layer,
                        PositionX = mapObject.GetPosition().X,
                        PositionY = mapObject.GetPosition().Y,
                        Properties = mapObject.Properties,
                        Info = mapObject.Info,
                        Name = mapObject.Name,
                        Components = mapObject.Components
                    });
                }
            }

            return map;
        }

        private void MenuItem_Export(object sender, RoutedEventArgs e)
        {
            SaveFileDialog projectDialog = new SaveFileDialog();
            projectDialog.Filter = "Pest Control Map Project (*.pcmap)|*.pcmap";

            if (projectDialog.ShowDialog() == true)
            {
                MapJSON map = GetCurrentMap();

                map.WriteBinaryMapFile(projectDialog.FileName);
            }
        }

        private void GridSplitter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gridDown = true;
        }

        private void GridSplitter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            gridDown = false;

            Viewport.UpdateSize();
        }

        private void Viewport_KeyDown(object sender, KeyEventArgs e)
        {
            MainWindowViewModel.viewModel.WPFKeyDown(e);
        }

        private void GridCheckBox_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.viewModel.GridVisible = (bool)GridCheckBox.IsChecked;
        }

        private void GridSizeTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            bool isInt;

            isInt = int.TryParse(GridSizeTextBox.Text, out int output);

            if (!isInt)
            {
                GridSizeTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(181, 74, 47));
            }
            else
            {
                GridSizeTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(189, 195, 199));

                MainWindowViewModel.viewModel.GridSize = output;
            }
        }

        
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                object obj = System.Windows.Media.ColorConverter.ConvertFromString(BackColorTextBox.Text);

                if (obj != null)
                {
                    System.Windows.Media.Color? color = obj as System.Windows.Media.Color?;

                    if (color != null)
                    {
                        MainWindowViewModel.viewModel.BackgroundColor = new Color(color.Value.R, color.Value.G, color.Value.B);

                        BackColorTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(189, 195, 199));
                    }
                }
            }
            catch(FormatException)
            {
                BackColorTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(181, 74, 47));
            }
        }

        // Exit
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Edit > Undo
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.viewModel.Undo();
        }

        // Edit > Redo
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.viewModel.Redo();
        }
    }
}
