using PestControlMapper.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Mapping
{
    public class ObjectInformationFile
    {
        public List<GameObjectInfo> ObjectInfos { get; set; }

        public List<ComponentInfo> ComponentInfos { get; set; }

        public ObjectInformationFile()
        {
            ObjectInfos = new List<GameObjectInfo>();
        }
    }
}
