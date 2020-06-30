using PestControlAnimation.Objects;
using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Mapping
{
    public class GameObjectInfo
    {
        public GameObjectInfo()
        {
            Properties = new Dictionary<string, GameObjectProperty>();
            HelperRectangles = new List<HelperRectangle>();
        }

        public Animation DefaultAnimation { get; set; } = new Animation();

        public string ClassName { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public Dictionary<string, GameObjectProperty> Properties { get; set; }

        public string AnimationProperty { get; set; } = "";

        public List<HelperRectangle> HelperRectangles { get; set; }
        public GameObjectInfo Copy()
        {
            GameObjectInfo newCopy = new GameObjectInfo();


            // Copy properties
            newCopy.Properties = new Dictionary<string, GameObjectProperty>();

            foreach(KeyValuePair<string, GameObjectProperty> pair in Properties)
            {
                GameObjectProperty newProperty = new GameObjectProperty(string.Copy(pair.Value.RealName), pair.Value.propertyType);
                newProperty.CurrentValue = string.Copy(pair.Value.CurrentValue);
                newProperty.DefaultValue = string.Copy(pair.Value.DefaultValue);

                newCopy.Properties.Add(string.Copy(pair.Key), newProperty);
            }

            newCopy.DefaultAnimation = DefaultAnimation.Copy();
            newCopy.ClassName = string.Copy(ClassName);
            newCopy.AnimationProperty = string.Copy(AnimationProperty);
            newCopy.DisplayName = string.Copy(DisplayName);
            newCopy.HelperRectangles = HelperRectangles;

            return newCopy;
        }
    }
}
