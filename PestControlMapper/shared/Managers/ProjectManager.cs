using Newtonsoft.Json;
using PestControlEngine.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.shared.Managers
{
    public class ProjectManager
    {
        public ObjectInformationFile objectInformation = new ObjectInformationFile();

        public string ProjectPath = string.Empty;

        public void LoadObjectInfos(string path)
        {
            string input = File.ReadAllText(path);
            objectInformation = JsonConvert.DeserializeObject<ObjectInformationFile>(input);
            foreach(GameObjectInfo info in objectInformation.ObjectInfos)
            {
                Console.WriteLine(info.ClassName);
                Console.WriteLine(info.DisplayName);
            }
        }
    }
}
