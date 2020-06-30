using Microsoft.Xna.Framework;
using PestControlEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlMapper.shared.clipboard
{
    public class ClipboardObjectMove : IClipboardItem
    {
        public List<Vector2> OldPositions { get; set; }
        public List<Vector2> NewPositions { get; set; }

        public List<GameObject> MovedObjects { get; set; }

        public ClipboardObjectMove(List<Vector2> oldPos, List<Vector2> newPos, List<GameObject> objs)
        {
            OldPositions = oldPos;
            NewPositions = newPos;
            MovedObjects = objs;
        }

        public void Redo()
        {
            if (MovedObjects != null)
            {
                for(int i = 0; i < MovedObjects.Count; i++)
                {
                    GameObject currentObject = MovedObjects[i];

                    currentObject.SetPosition(NewPositions[i]);
                }
            }
        }

        public void Undo()
        {
            if (MovedObjects != null)
            {
                for(int i = 0; i < MovedObjects.Count; i++)
                {
                    GameObject currentObject = MovedObjects[i];

                    Console.WriteLine("undo");

                    currentObject.SetPosition(OldPositions[i]);
                }
            }
        }
    }
}
