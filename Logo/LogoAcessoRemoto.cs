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
    public partial class LogoAcessoRemoto : Form
    {
        public LogoAcessoRemoto()
        {
            InitializeComponent();
        }

        private void LogoAcessoRemoto_Load(object sender, EventArgs e)
        {
            Location = new Point(Screen.PrimaryScreen.Bounds.Width - 170, 80);
        }
    }
}
