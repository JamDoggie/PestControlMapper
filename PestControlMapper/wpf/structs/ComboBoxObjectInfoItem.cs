using PestControlEngine.Mapping;
using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.wpf.structs
{
    public class ComboBoxObjectInfoItem
    {
        public string RealName { get; set; }

        public GameObjectInfo GameObject { get; set; }


        public override string ToString()
        {
            return RealName;
        }
    }
}
