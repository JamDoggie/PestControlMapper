using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PestControlEngine.GameManagers;
using PestControlEngine.GUI;
using PestControlEngine.Libs.Helpers.Structs;
using PestControlEngine.Mapping;
using PestControlEngine.Objects;
using PestControlEngine.Resource;
using PestControlMapper.GUI;
using PestControlMapper.MonoGameControls;
using PestControlMapper.Objects;
using PestControlMapper.shared.clipboard;
using PestControlMapper.shared.Enum;
using PestControlMapper.wpf.controls;
using PestControlMapper.wpf.structs;

namespace PestControlMapper
{
    public class MainWindowViewModel : MonoGameViewModel
    {
        private SpriteBatch _spriteBatch;

        private SpriteBatch _guiBatch;

        public ObjectManager objectManager = new ObjectManager();

        public GUIManager guiManager = new GUIManager();

        public static MainWindowViewModel viewModel = null;

        public int GridSize { get; set; } = 32;
        public bool GridVisible { get; set; } = true;

        public Color BackgroundColor { get; set; } = new Color(60, 60, 60);

        public SelectionType SelectionType { get; set; }

        public int ClipboardSize { get; set; } = 100;

        public List<IClipboardItem> ClipBoardItems = new List<IClipboardItem>();

        public int ClipBoardOffset = 0;


        public MainWindowViewModel()
        {
            viewModel = this;
        }
        UIImageButton objectSelectionButton;
        UIImageButton selectionButton;

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _guiBatch = new SpriteBatch(GraphicsDevice);

            ContentLoader.LoadFonts(Content);
            ContentLoader.LoadTextures(Content);
            ContentLoader.LoadShaders(Content);
            ContentLoader.LoadSounds(Content);

            Screen editorScreen = new Screen();
            guiManager.LoadScreen("editor_screen", editorScreen);
            guiManager.SetScreen("editor_screen");

            selectionButton = new UIImageButton(new Vector2(8, 8), 32, 32);
            selectionButton.Image = "textures/tools/selection_icon";
            selectionButton.IsOption = true;
            editorScreen.AddControl(selectionButton);
            selectionButton.IsChosenOption = true;

            selectionButton.MouseClickedEvent += SelectionButton_MouseClickedEvent;

            objectSelectionButton = new UIImageButton(new Vector2(44, 8), 32, 32);
            objectSelectionButton.Image = "textures/tools/object_selection_icon";
            objectSelectionButton.IsOption = true;
            editorScreen.AddControl(objectSelectionButton);

            UIFPSCounter fpsCounter = new UIFPSCounter();
            fpsCounter.Position.Y = 50;
            editorScreen.AddControl(fpsCounter);

            objectSelectionButton.MouseClickedEvent += ObjectSelectionButton_MouseClickedEvent;

            UpdateSelection();
        }

        private void ObjectSelectionButton_MouseClickedEvent(System.Windows.Input.MouseEventArgs e)
        {
            objectSelectionButton.IsChosenOption = true;
            selectionButton.IsChosenOption = false;
            SelectionType = SelectionType.OBJECT;
        }

        private void SelectionButton_MouseClickedEvent(System.Windows.Input.MouseEventArgs e)
        {
            selectionButton.IsChosenOption = true;
            objectSelectionButton.IsChosenOption = false;
            SelectionType = SelectionType.SELECTION;
        }

        public override void Update(GameTime gameTime)
        {
            GameInfo info = new GameInfo(GraphicsDevice, _spriteBatch, objectManager, guiManager, GetResolution(), Content, gameTime);
            objectManager.Update(gameTime, info);
            guiManager.Update(gameTime, info);


            if (SelectionType == SelectionType.OBJECT)
                objectManager.PreviewObject.Visible = true;
            else
                objectManager.PreviewObject.Visible = false;

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            GameInfo info = new GameInfo(GraphicsDevice, _spriteBatch, objectManager, guiManager, GetResolution(), Content, gameTime);
            objectManager.Draw(GraphicsDevice, _spriteBatch, info);
            guiManager.Draw(gameTime, GraphicsDevice, _guiBatch, info);
        }

        public void UpdateObjectTree()
        {
            MainWindow.mainWindow.ObjectTree.Children.Clear();

            foreach(GameObject obj in objectManager.GetObjects())
            {
                MapGameObject mapObject = obj as MapGameObject;

                if (mapObject != null)
                {
                    CheckBoxPair checkPair = new CheckBoxPair();
                    string name = "";

                    foreach(KeyValuePair<string, GameObjectProperty> property in mapObject.Properties)
                    {
                        if (property.Key == "Name")
                        {
                            name = property.Value.CurrentValue;
                        }
                    }

                    checkPair.MainLabel.Text = name;

                    checkPair.MainCheckBox.IsChecked = mapObject.Visible;

                    checkPair.MainCheckBox.Click += (sender, e) =>
                    {
                        mapObject.Visible = (bool)checkPair.MainCheckBox.IsChecked;
                    };

                    MainWindow.mainWindow.ObjectTree.Children.Add(checkPair);
                }
            }
        }

        public void UpdateSelection()
        {
            ComboBox comboBox = MainWindow.mainWindow.ObjectComboBox;


            if (comboBox.SelectedItem != null && SelectionType == SelectionType.OBJECT)
            {
                ComboBoxObjectInfoItem item = comboBox.SelectedItem as ComboBoxObjectInfoItem;

                if (item != null)
                {
                    objectManager.PreviewObject.CurrentAnimation = item.GameObject.DefaultAnimation;

                    objectManager.PreviewObject.Properties = item.GameObject.Properties;

                    objectManager.PreviewObject.Info = item.GameObject;
                }
            }
        }

        private Dictionary<GameObject, Vector2> grabOffset = new Dictionary<GameObject, Vector2>();

        private bool isMoving = false;

        private List<Vector2> OldPositions = new List<Vector2>();


        public void WPFMouseMove(MouseEventArgs e)
        {
            objectManager.PreviewObject.SetPosition(Vector2.Transform(new Vector2((float)e.GetPosition(MainWindow.mainWindow.Viewport).X, (float)e.GetPosition(MainWindow.mainWindow.Viewport).Y), Matrix.Invert(objectManager.CurrentCamera.Transform)));

            Vector2 worldPos = Vector2.Transform(new Vector2((float)e.GetPosition(MainWindow.mainWindow.Viewport).X, (float)e.GetPosition(MainWindow.mainWindow.Viewport).Y), Matrix.Invert(objectManager.CurrentCamera.Transform));

            if (objectManager.SelectionObject.Selection != null)
            {
                foreach(GameObject gameObject in objectManager.SelectionObject.Selection)
                {
                    if (e.LeftButton == MouseButtonState.Pressed && SelectionType == SelectionType.SELECTION && grabOffset.TryGetValue(gameObject, out _))
                    {
                        gameObject.SetPosition(new Vector2((int)worldPos.X, (int)worldPos.Y) - new Vector2((int)grabOffset[gameObject].X, (int)grabOffset[gameObject].Y));
                    }
                }
            }
            
        }

        public void WPFMouseDown(MouseEventArgs e)
        {
            ComboBox comboBox = MainWindow.mainWindow.ObjectComboBox;

            ComboBoxObjectInfoItem item = comboBox.SelectedItem as ComboBoxObjectInfoItem;
            Vector2 worldPos = Vector2.Transform(new Vector2((float)e.GetPosition(MainWindow.mainWindow.Viewport).X, (float)e.GetPosition(MainWindow.mainWindow.Viewport).Y), Matrix.Invert(objectManager.CurrentCamera.Transform));

            // Placing objects
            if (e.LeftButton == MouseButtonState.Pressed && SelectionType == SelectionType.OBJECT && item != null && !guiManager.UIHovered)
            {
                MapGameObject mapObject = new MapGameObject(item.GameObject.Copy());

                if (item.GameObject == null)
                    Console.WriteLine("GameObjectInfo is null!");

                mapObject.SetPosition(objectManager.PreviewObject.GetPosition());

                mapObject.CurrentAnimation = objectManager.PreviewObject.CurrentAnimation;
                mapObject.Properties = item.GameObject.Properties;
                mapObject.Info = item.GameObject.Copy();
                mapObject.HelperRectangles = item.GameObject.HelperRectangles;

                objectManager.GetObjects().Add(mapObject);

                UpdateObjectTree();
            }

            // Selecting objects
            if (e.LeftButton == MouseButtonState.Pressed && SelectionType == SelectionType.SELECTION && !guiManager.UIHovered)
            {
                // Ensure the mouse is not already over a selected object.
                bool isSelectionHovered = false;

                foreach(GameObject obj in objectManager.SelectionObject.Selection)
                {
                    if (obj.GetBoundingBox().Intersects(new Rectangle((int)worldPos.X, (int)worldPos.Y, 1, 1)))
                    {
                        isSelectionHovered = true;
                    }
                }

                if (!isSelectionHovered || (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) )
                {
                    if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                    {
                        objectManager.SelectionObject.Selection = new List<GameObject>();
                    }
                        

                    foreach (GameObject obj in objectManager.GetObjects())
                    {
                        if (new Rectangle((int)worldPos.X, (int)worldPos.Y, 1, 1).Intersects(obj.GetBoundingBox()) && !objectManager.SelectionObject.Selection.Contains(obj) && obj != objectManager.PreviewObject && obj.Visible)
                        {
                            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                            {
                                if (objectManager.SelectionObject.Selection.Count == 0)
                                {
                                    objectManager.SelectionObject.Selection.Add(obj);
                                }
                                    
                                else
                                {
                                    objectManager.SelectionObject.Selection[0] = obj;
                                }
                            }
                            else
                            {
                                objectManager.SelectionObject.Selection.Add(obj);
                            }

                            GameObjectAnimated animObj = objectManager.SelectionObject.Selection[0] as GameObjectAnimated;

                            if (animObj != null && animObj.CurrentAnimation != null)
                            {
                                animObj.CurrentAnimation.SetCurrentMS(0);
                                animObj.CurrentAnimation.Play();
                            }
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
                UpdateLayerBox();

            if (e.LeftButton == MouseButtonState.Pressed && objectManager.SelectionObject.Selection.Count > 0)
            {
                OldPositions = new List<Vector2>();

                foreach(GameObject obj in objectManager.SelectionObject.Selection)
                {
                    OldPositions.Add(obj.GetPosition());
                }

                isMoving = true;
            }

            /// OBJECT MOVING

            // Get relative position to origin of selection
            if (objectManager.SelectionObject.Selection != null)
            {
                foreach(GameObject gameObject in objectManager.SelectionObject.Selection)
                {
                    if (gameObject is MapGameObject)
                    {
                        grabOffset[gameObject] = worldPos - gameObject.GetPosition();
                    }
                }
            }
                
        }

        public void WPFMouseUp(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                isMoving = false;

                bool posDifferent = false;

                List<Vector2> NewPositions = new List<Vector2>();

                for (int i = 0; i < objectManager.SelectionObject.Selection.Count; i++)
                {
                    GameObject obj = objectManager.SelectionObject.Selection[i];

                    if (obj.GetPosition() != OldPositions[i])
                    {
                        posDifferent = true;

                        NewPositions.Add(obj.GetPosition());
                    }
                }

                if (posDifferent)
                {
                    if (ClipBoardOffset != 0)
                    {
                        ClipBoardItems.Clear();
                        ClipBoardOffset = 0;
                    }
                        

                    ClipBoardItems.Add(new ClipboardObjectMove(OldPositions, NewPositions, objectManager.SelectionObject.Selection));

                    if (ClipBoardItems.Count > ClipboardSize)
                    {
                        ClipBoardItems = ClipBoardItems.GetRange(ClipBoardItems.Count - ClipboardSize, ClipboardSize);
                    }
                }
            }
                
        }

        public void Undo()
        {
            if (ClipBoardItems.Count > 0)
            {
                ClipBoardItems[(ClipBoardItems.Count - 1) + ClipBoardOffset].Undo();

                if ((ClipBoardItems.Count - 1) + (ClipBoardOffset - 1) >= 0)
                {
                    ClipBoardOffset--;
                }
            }
        }

        public void Redo()
        {
            if (ClipBoardItems.Count > 0)
            {
                ClipBoardItems[(ClipBoardItems.Count - 1) + ClipBoardOffset].Redo();

                if (ClipBoardOffset < 0)
                {
                    ClipBoardOffset++;
                }
            }
        }

        public void WPFKeyDown(KeyEventArgs e)
        {
            

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // Undo
                if (Keyboard.IsKeyDown(Key.Z))
                {
                    Undo();
                }

                // Redo
                if (Keyboard.IsKeyDown(Key.Y))
                {
                    Redo();
                }
            }
        }

        public void UpdateLayerBox()
        {
            MainWindow mainWindow = MainWindow.mainWindow;
            if (objectManager.SelectionObject.Selection != null && objectManager.SelectionObject.Selection.Count > 0)
            {
                if (objectManager.SelectionObject.Selection.Count == 0)
                    return;



                mainWindow.LayerTextBox.IsEnabled = true;

                bool allSameLayer = true;
                float? lastLayer = null;

                // Check if all selected objects have the same layer
                foreach (GameObject obj in objectManager.SelectionObject.Selection)
                {
                    if (lastLayer == null)
                    {
                        lastLayer = obj.Layer;

                        continue;
                    }

                    if (obj.Layer != lastLayer)
                    {
                        allSameLayer = false;

                        break;
                    }

                    lastLayer = obj.Layer;
                }

                if (allSameLayer && lastLayer != null)
                    mainWindow.LayerTextBox.Text = ((float)lastLayer).ToString();
                else
                {
                    mainWindow.LayerTextBox.Text = "[different]";
                    mainWindow.LayerTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(181, 74, 47));
                }
                    

                // change layer when textbox is typed in
                mainWindow.LayerTextBox.TextChanged += (sender, e) =>
                {
                    // If valid float, update layer and change color to default.
                    if (float.TryParse(mainWindow.LayerTextBox.Text, out float outParse))
                    {
                        // ( thanks XNA for having classes that are the same name as classes from .net and WPF :} )
                        mainWindow.LayerTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(189, 195, 199));

                        foreach(GameObject obj in objectManager.SelectionObject.Selection)
                        {
                            obj.Layer = outParse;
                        }
                    }
                    else
                    {
                        // Turn text red and don't update
                        mainWindow.LayerTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(181, 74, 47));
                    }
                };
            }
            else
            {
                mainWindow.LayerTextBox.IsEnabled = false;
            }
        }

        public Vector2 GetResolution()
        {
            return new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }
    }
}