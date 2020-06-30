using Microsoft.Xna.Framework;
using PestControlEngine.GUI;

namespace PestControlEngine.Libs.Helpers
{
    public static class Util
    {
        public static string GetEngineNull()
        {
            return $"{GetEnginePrefix()}null";
        }

        public static string GetEnginePrefix()
        {
            return "enginereserved_";
        }
    }


}
