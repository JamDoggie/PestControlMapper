using Newtonsoft.Json;
using PestControlEngine.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PestControlMapper.shared.Managers
{
    public static class PreferencesConfiguration
    {
        public static PreferencesJson Preferences = new PreferencesJson();

        public static string PreferencesPath = "preferences.json";

        public static void LoadFromFile(string path)
        {
            PreferencesJson json = JsonConvert.DeserializeObject<PreferencesJson>(File.ReadAllText(path));

            Preferences = json;
        }

        public static void SaveToFile(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Preferences, Formatting.Indented));
        }
    }

    public class PreferencesJson
    {
        public List<GameConfiguration> GameConfigs { get; set; } = new List<GameConfiguration>();

        public string SelectedConfig { get; set; } = "";
    }

    public class GameConfiguration
    {
        public string GameName { get; set; }

        public string ContentPath { get; set; }

        public override string ToString()
        {
            return GameName;
        }
    }
}
