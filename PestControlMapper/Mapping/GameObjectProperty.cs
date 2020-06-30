using PestControlEngine.Mapping.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Mapping
{
    public class GameObjectProperty
    {
        // Name that shows up in the map editor. The actual internal variable name is stored as the key in the parent dictionary.
        public string RealName { get; set; }

        /* 
         * Since this value can be any type, it is stored as a string that can be parsed.
         * It is mainly stored this way instead of directly as a generic type for easy serialization.
        */

        public string DefaultValue { get; set; }

        private string currentValue = null;
        public string CurrentValue
        {
            get
            {
                if (currentValue == null)
                    return DefaultValue;

                return currentValue;
            }

            set { currentValue = value; }
        }

        public PropertyType propertyType { get; set; }

        public GameObjectProperty(string realName, PropertyType type)
        {
            RealName = realName;

            SetType(type);
        }

        public void SetType(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.INT:
                    DefaultValue = 0.ToString();
                    propertyType = PropertyType.INT;
                    break;
                case PropertyType.FLOAT:
                    DefaultValue = 0.0f.ToString();
                    propertyType = PropertyType.FLOAT;
                    break;
                case PropertyType.DOUBLE:
                    DefaultValue = 0.0d.ToString();
                    propertyType = PropertyType.DOUBLE;
                    break;
                case PropertyType.STRING:
                    DefaultValue = "";
                    propertyType = PropertyType.STRING;
                    break;
                case PropertyType.BOOL:
                    DefaultValue = false.ToString();
                    propertyType = PropertyType.BOOL;
                    break;
            }
        }

        /// <summary>
        /// Attempts to read and return the current value as an int32.
        /// </summary>
        /// <returns>(Nullable)Integer</returns>
        public int? GetAsInt32()
        {
            int.TryParse(CurrentValue, out int output);

            return output;
        }

        /// <summary>
        /// Attempts to read and return the current value as a float.
        /// </summary>
        /// <returns>(Nullable)Float</returns>
        public float? GetAsFloat()
        {

            float.TryParse(CurrentValue, out float output);

            return output;
        }

        /// <summary>
        /// Attempts to read and return the current value as a double. 
        /// </summary>
        /// <returns>(Nullable)Double</returns>
        public double? GetAsDouble()
        {
            double.TryParse(CurrentValue, out double output);
            return output;
        }

        /// <summary>
        /// Attempts to read and return the current value as a string. 
        /// </summary>
        /// <returns>(Nullable)String</returns>
        public string GetAsString()
        {
            return CurrentValue;
        }

        /// <summary>
        /// Attempts to read and return the current value as a boolean. 
        /// </summary>
        /// <returns>(Nullable)Boolean</returns>
        public bool? GetAsBool()
        {
            bool.TryParse(CurrentValue, out bool output);
            return output;
        }

        public void SetValue(int value)
        {
            CurrentValue = value.ToString();
            SetType(PropertyType.INT);
        }

        public void SetValue(float value)
        {
            CurrentValue = value.ToString();
            SetType(PropertyType.FLOAT);
        }

        public void SetValue(double value)
        {
            CurrentValue = value.ToString();
            SetType(PropertyType.DOUBLE);
        }

        public void SetValue(string value)
        {
            CurrentValue = value;
            SetType(PropertyType.STRING);
        }

        public void SetValue(bool value)
        {
            CurrentValue = value.ToString();
            SetType(PropertyType.BOOL);
        }
    }
}
