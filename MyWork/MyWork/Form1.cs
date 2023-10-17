using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyWork
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            if(pictureBox1.Image == null)
            {
                pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            }

            var graphics = Graphics.FromImage(pictureBox1.Image);

            graphics.Clear(Color.Yellow);

            if ( "Circle".Equals(textBox1.Text))
            {
                graphics.FillEllipse(Brushes.Aquamarine, 10, 10, 100, 100);
            }

            if ("Rectangle".Equals(textBox1.Text))
            {
                graphics.FillRectangle(Brushes.Aquamarine, 10, 10, 100, 100);
            }
            // MessageBox.Show("Hello");
            pictureBox1.Refresh();
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
