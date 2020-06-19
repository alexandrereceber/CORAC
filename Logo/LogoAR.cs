using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORAC.Logo
{
    public partial class LogoAR : Form
    {
        public LogoAR()
        {
            InitializeComponent();
        }

        private void LogoAR_Load(object sender, EventArgs e)
        {
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 160, 100);


        }



        private void LogoAR_Paint(object sender, PaintEventArgs e)
        {

            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void LogoAR_Shown(object sender, EventArgs e)
        {

        }
    }
}
