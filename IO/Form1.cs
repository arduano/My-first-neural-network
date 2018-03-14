using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NNet;

namespace IO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DrawGraphics dg;

        public bool ready = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            dg = new DrawGraphics(this, false);
        }

        public void DrawNN(double[][] layers, int xo, int yo)
        {
            dg.Start();
            //dg.Clear(Color.White);
            for (int i = 0; i < layers.Length; i++)
            {
                for (int j = 0; j < layers[i].Length; j++)
                {
                    int x = i * 40 + xo;
                    int y = j * 24 + yo;
                    dg.DrawEllipse(x, y, 24, 24, Color.LightGray, Color.Transparent, 0);
                    StringFormat sf = StringFormat.GenericDefault;
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    dg.DrawText(x + 12, y + 12, Color.Black, "Arial", 8, (Math.Round(layers[i][j] * 10) / 10).ToString(), sf);
                }
            }
            dg.Finish();
        }

        public void LiveNNet(Net net)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ready = true;
        }
    }
}
