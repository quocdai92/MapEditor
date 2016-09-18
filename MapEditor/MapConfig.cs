using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class MapConfig : Form
    {
        public int width = 0;
        public int height = 0;
        public MapConfig()
        {
            InitializeComponent();
            numWidth.Value = Map.width;
            numHeight.Value = Map.height;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Map.width = Convert.ToInt32(numWidth.Value);
            Map.height = Convert.ToInt32(numHeight.Value);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            width = Convert.ToInt32(numWidth.Value);
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            height = Convert.ToInt32(numHeight.Value);
        }

        private void numWidth_Enter(object sender, EventArgs e)
        {
            numWidth.Select(0, numWidth.Text.Length);
        }

        private void numHeight_Enter(object sender, EventArgs e)
        {
            numHeight.Select(0,numHeight.Text.Length);
        }
    }
}
