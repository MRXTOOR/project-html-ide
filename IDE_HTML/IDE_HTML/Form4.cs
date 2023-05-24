using System;
using System.Windows.Forms;

namespace IDE_HTML
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent(); this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
