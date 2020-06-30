using PestControlEngine.Libs.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Libs.Localization
{
    public static class Localization
    {
        public const string langPath = "bin/lang/";
        public static string currentLanguage = "en_US";
        public static string Translate(string key)
        {
            string translation = key;

            FileStream fileStream = new FileStream($"{langPath}{currentLanguage}.lang", FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            List<string> lines = new List<string>();

            lines = FileHelpers.ReadAllLinesStream(streamReader).ToList();

            foreach(string s in lines)
            {
                string line = s;

                // Remove any whitespace at the start or end of the line in the line object(does not modify the actual file)
                line.Trim();

                if (line[0] == '#')
                {
                    string[] segments = line.Split(new[] { ',' }, 2);

                    if (segments[0] == key && segments.Count() > 1)
                    {
                        translation = segments[1];
                    }
                }
            }

            return translation;
        }
    }
}
