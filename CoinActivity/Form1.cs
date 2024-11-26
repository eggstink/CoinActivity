using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CoinActivity
{
    public partial class Form1 : Form
    {
        Bitmap coinsImage;
        CoinReader coinReader;

        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            coinsImage = new Bitmap(openFileDialog1.FileName);
            if (coinsImage != null)
            {
                pictureBox1.Image = coinsImage;
                button1.Enabled = true;
                coinReader = new CoinReader(openFileDialog1.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = coinReader.ImagePreprocessing();

            var radiiList = coinReader.GetListRadii();
            radiiList.Sort();

            var coinCount = GroupCoins(radiiList);

            tb5cents.Text = coinCount["C5"].ToString();
            tb10cents.Text = coinCount["C10"].ToString();
            tb25cents.Text = coinCount["C25"].ToString();
            tb1pesos.Text = coinCount["P1"].ToString();
            tb5pesos.Text = coinCount["P5"].ToString();

            double total = CalculateTotal(coinCount);
            tbTotal.Text = total.ToString("0.00");
        }

        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            openFileDialog1.ShowDialog();
        }


        private Dictionary<string, int> GroupCoins(List<double> radii)
        {
            var coinCounts = new Dictionary<string, int>
            {
                { "C5", 0 }, { "C10", 0 }, { "C25", 0 }, { "P1", 0 }, { "P5", 0 }
            };

            string[] coinTypes = { "C5", "C10", "C25", "P1", "P5" };
            int currentCoinIndex = 0;  

            for (int i = 0; i < radii.Count; i++)
            {
                if (i > 0 && Math.Abs(radii[i] - radii[i - 1]) >= 0.8)
                currentCoinIndex++;
                coinCounts[coinTypes[currentCoinIndex]]++;
            }

            return coinCounts;
        }

        private double CalculateTotal(Dictionary<string, int> coinCount)
        {
            double total = 0;
            total += coinCount["C5"] * 0.05;
            total += coinCount["C10"] * 0.10;
            total += coinCount["C25"] * 0.25;
            total += coinCount["P1"] * 1.00;
            total += coinCount["P5"] * 5.00;
            return total;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
