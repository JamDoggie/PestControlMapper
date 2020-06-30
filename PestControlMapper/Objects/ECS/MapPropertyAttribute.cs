using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Objects.ECS
{
    public class MapPropertyAttribute : Attribute
    {
        public string RealName { get; set; }
        public string Info { get; set; }
        public object DefaultValue { get; set; }
        public MapPropertyAttribute(string realName, string info, object defaultValue = null)
        {
            RealName = realName;
            Info = info;
            DefaultValue = defaultValue;
        }
    }
}
