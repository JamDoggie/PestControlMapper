using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace PestControlMapper.wpf.structs
{
    public class ComboBoxComponentInfoItem
    {
        public string RealName { get; set; }

        public ComponentInfo ComponentInfo { get; set; }

        public override string ToString()
        {
            return RealName;
        }
    }
}
