using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORAC
{
    public partial class CORAC_TPrincipal : Form
    {
        public CORAC_TPrincipal()
        {
            InitializeComponent();
        }

        private void Status_Internet_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap Obj = (Bitmap)picture_Internet_CORAC.Image;
            for (var i = 0; i < Obj.Height; i++) 
            {
                for (var ii = 0; ii < Obj.Width; ii++)
                {
                   Color g = Obj.GetPixel(i, ii);
                    if(g.A == 255 && g.R==33 && g.G==117 && g.B == 170)
                    {
                        Obj.SetPixel(i, ii,Color.Red);
                    }
                }
            }
            picture_Internet_CORAC.Image = Obj;

        }


        private void picture_Status_CORAC_MouseClick(object sender, MouseEventArgs e)
        {
            //int p = e.X;
            //int c = e.Y;
        }
    }
}
