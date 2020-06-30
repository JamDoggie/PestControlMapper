using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.shared.clipboard
{
    public interface IClipboardItem
    {
        void Undo();
        void Redo();
    }
}
