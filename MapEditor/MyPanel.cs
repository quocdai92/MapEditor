using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    public class MyPanel : Panel
    {
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        public MyPanel()
        {
            //this.SetStyle(
            //    ControlStyles.UserPaint |
            //    ControlStyles.AllPaintingInWmPaint |
            //    ControlStyles.OptimizedDoubleBuffer,
            //    true);
        }
    }
}
