using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.Mapping
{
    public struct HelperRectangle
    {
        public string PosXProperty { get; set; }

        public string PosYProperty { get; set; }

        public string WidthProperty { get; set; }

        public string HeightProperty { get; set; }

        public HelperRectangle Copy()
        {
            var rect = new HelperRectangle();
            rect.PosXProperty = (string)PosXProperty.Clone();
            rect.PosYProperty = (string)PosYProperty.Clone();
            rect.WidthProperty = (string)WidthProperty.Clone();
            rect.HeightProperty = (string)HeightProperty.Clone();

            return rect;
        }
    }
}
