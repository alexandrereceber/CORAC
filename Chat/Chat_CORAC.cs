using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CORAC.Chat
{
    public partial class Chat_CORAC : Form
    {
        public Chat_CORAC()
        {
            InitializeComponent();
        }

        private void Chat_CORAC_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
        }

        private void Chat_CORAC_Leave(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
