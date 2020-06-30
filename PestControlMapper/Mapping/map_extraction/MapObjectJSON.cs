using PestControlEngine.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Mapping.map_extraction
{
    public class MapObjectJSON
    {
        public Dictionary<string, GameObjectProperty> Properties { get; set; }

        public List<ComponentInfo> Components { get; set; }

        public float Layer { get; set; } = 0.0f;

        public float PositionX { get; set; } = 0.0f;

        public float PositionY { get; set; } = 0.0f;

        public GameObjectInfo Info { get; set; } = new GameObjectInfo();

        public string Name { get; set; } = "";

        public MapObjectJSON()
        {
            Properties = new Dictionary<string, GameObjectProperty>();
            Components = new List<ComponentInfo>();
        }
    }
}
