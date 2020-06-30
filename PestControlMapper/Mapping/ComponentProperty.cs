using PestControlEngine.Mapping;
using PestControlEngine.Mapping.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Mapping
{
    public class ComponentProperty : GameObjectProperty
    {
        public string TooltipInfo { get; set; }

        public ComponentProperty(string realName, PropertyType type, string info = "") : base(realName, type)
        {
            TooltipInfo = info;
        }

        public ComponentProperty Copy()
        {
            ComponentProperty componentProperty = new ComponentProperty(RealName, propertyType, TooltipInfo);
            componentProperty.CurrentValue = CurrentValue;
            componentProperty.DefaultValue = DefaultValue;

            return componentProperty;
        }
    }
}
