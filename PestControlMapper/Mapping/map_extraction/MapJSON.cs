using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers;
using PestControlEngine.Mapping;
using PestControlEngine.Mapping.Enums;
using PestControlEngine.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Mapping.map_extraction
{
    public class MapJSON
    {
        // File format consts
        public const string HeaderString = "PestControlBinaryMapFile";
        public const int HeaderVersion = 1;

        // File Variables
        public List<MapObjectJSON> Objects { get; set; }

        public byte BackgroundR { get; set; } = 60;
        public byte BackgroundG { get; set; } = 60;
        public byte BackgroundB { get; set; } = 60;

        public MapJSON()
        {
            Objects = new List<MapObjectJSON>();
        }

        /// <summary>
        /// Writes .pcmap file to the specified path. This contains informations about objects and textures used in a binary format.
        /// </summary>
        /// <param name="path"></param>
        public void WriteBinaryMapFile(string path)
        {
            FileStream stream = File.Open(path, FileMode.OpenOrCreate);
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            /// HEADER ///
            binaryWriter.Write(HeaderString);
            binaryWriter.Write(HeaderVersion);

            /// MAP PROPERTIES ///

            binaryWriter.Write(BackgroundR);
            binaryWriter.Write(BackgroundG);
            binaryWriter.Write(BackgroundB);

            /// OBJECTS ///
            binaryWriter.Write(Objects.Count);

            foreach (MapObjectJSON obj in Objects)
            {
                binaryWriter.Write(obj.Name);
                binaryWriter.Write(obj.Layer);
                binaryWriter.Write(obj.PositionX);
                binaryWriter.Write(obj.PositionY);

                // Object creation information
                binaryWriter.Write(obj.Info.ClassName);

                // Object UUID
                binaryWriter.Write(Guid.NewGuid().ToString());

                // Properties
                binaryWriter.Write(obj.Properties.Count);

                foreach(KeyValuePair<string, GameObjectProperty> pair in obj.Properties)
                {
                    // Property name
                    binaryWriter.Write(pair.Key);

                    // Property value
                    binaryWriter.Write((int)pair.Value.propertyType);

                    switch(pair.Value.propertyType)
                    {
                        case PropertyType.INT:
                            binaryWriter.Write((int)pair.Value.GetAsInt32());
                            break;
                        case PropertyType.BOOL:
                            binaryWriter.Write((bool)pair.Value.GetAsBool());
                            break;
                        case PropertyType.DOUBLE:
                            binaryWriter.Write((double)pair.Value.GetAsDouble());
                            break;
                        case PropertyType.FLOAT:
                            binaryWriter.Write((float)pair.Value.GetAsFloat());
                            break;
                        case PropertyType.STRING:
                            binaryWriter.Write(pair.Value.GetAsString());
                            break;
                    }
                }

                // Components
                binaryWriter.Write(obj.Components.Count);

                foreach(ComponentInfo component in obj.Components)
                {
                    // Class name used to load component
                    binaryWriter.Write(component.ClassName);

                    // Component UUID
                    binaryWriter.Write(Guid.NewGuid().ToString());

                    // Component Properties
                    binaryWriter.Write(component.Properties.Count);

                    foreach (KeyValuePair<string, ComponentProperty> pair in component.Properties)
                    {
                        // Property name
                        binaryWriter.Write(pair.Key);
                        
                        // Property value
                        binaryWriter.Write((int)pair.Value.propertyType);

                        switch (pair.Value.propertyType)
                        {
                            case PropertyType.INT:
                                binaryWriter.Write((int)pair.Value.GetAsInt32());
                                break;
                            case PropertyType.BOOL:
                                binaryWriter.Write((bool)pair.Value.GetAsBool());
                                break;
                            case PropertyType.DOUBLE:
                                binaryWriter.Write((double)pair.Value.GetAsDouble());
                                break;
                            case PropertyType.FLOAT:
                                binaryWriter.Write((float)pair.Value.GetAsFloat());
                                break;
                            case PropertyType.STRING:
                                binaryWriter.Write(pair.Value.GetAsString());
                                break;
                        }
                    }
                }
            }

            /// ASSET INFORMATION (directory tree of any used assets such as textures for the game to pre-load)

            List<string> assetKeys = new List<string>();

            foreach(MapObjectJSON obj in Objects)
            {
                if (obj.Info.AnimationProperty != Util.GetEngineNull())
                {
                    Animation animation = ContentLoader.GetAnimation(obj.Properties[obj.Info.AnimationProperty].CurrentValue);

                    if (animation != null)
                    {
                        foreach(KeyFrame keyFrame in animation.KeyFrames)
                        {
                            foreach(KeyValuePair<string, SpriteJson> spritePair in keyFrame.SpriteBoxes)
                            {
                                if (!assetKeys.Contains(spritePair.Value.textureKey))
                                    assetKeys.Add(spritePair.Value.textureKey);
                            }
                        }
                    }
                }

                foreach(ComponentInfo component in obj.Components)
                {
                    if (component.AnimationProperty != Util.GetEngineNull())
                    {
                        Animation animation = ContentLoader.GetAnimation(component.Properties[component.AnimationProperty].CurrentValue);

                        if (animation != null)
                        {
                            foreach (KeyFrame keyFrame in animation.KeyFrames)
                            {
                                foreach (KeyValuePair<string, SpriteJson> spritePair in keyFrame.SpriteBoxes)
                                {
                                    if (!assetKeys.Contains(spritePair.Value.textureKey))
                                        assetKeys.Add(spritePair.Value.textureKey);
                                }
                            }
                        }
                    }
                }
            }

            binaryWriter.Write(assetKeys.Count);

            foreach(string assetKey in assetKeys)
            {
                binaryWriter.Write(assetKey);
            }


            binaryWriter.Dispose();
            stream.Dispose();
        }
    }
}
