using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Libs.Helpers
{
    public static class FileHelpers
    {
        /// <summary>
        /// Helper function, imitates System.IO.File.ReadAllLines but for a StreamReader.
        /// </summary>
        /// <param name="streamReader"></param>
        /// <returns></returns>
        public static string[] ReadAllLinesStream(StreamReader streamReader)
        {
            List<string> lines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                lines.Add(streamReader.ReadLine());
            }
            return lines.ToArray();
        }
    }
}
