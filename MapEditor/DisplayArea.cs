using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public class DisplayArea
    {
        public DisplayArea()
        {
            ListGrid = new List<Cell>();
        }
        public List<Cell> ListGrid { get; set; } 
        public Color Color { get; set; }
    }

    public class Cell
    {
        public int Size { get; set; }
        public Point StartPosition { get; set; }
        public Color Color { get; set; }
        //pixel location x
        public int X { get; set; }
        //pixel location y
        public int Y { get; set; }
        public bool IsEmpty { get; set; }
    }
}
