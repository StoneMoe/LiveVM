using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveVM
{
    public partial class setting : Form
    {
        public setting()
        {
            InitializeComponent();
        }

        private void setting_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LiveVM.ip = IPAddress.Parse(textBox1.Text);
            LiveVM.port = Convert.ToInt32(textBox2.Text);
            this.Close();
        }
    }
}
