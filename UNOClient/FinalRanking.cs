using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnoOnline
{
    public partial class FinalRanking : Form
    {
        private static FinalRanking instance;

        public static FinalRanking Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new FinalRanking();
                }
                return instance;
            }
        }

        private FinalRanking()
        {
            InitializeComponent();
        }

        public void DisplayRanking(List<Player> players)
        {
            // Clear previous entries
            NameTop1.Text = string.Empty;
            Point1.Text = string.Empty;
            NameTop2.Text = string.Empty;
            Point2.Text = string.Empty;
            NameTop3.Text = string.Empty;
            Point3.Text = string.Empty;
            NameTop4.Text = string.Empty;
            Point4.Text = string.Empty;

            // Sort players by points in descending order
            players.Sort((x, y) => y.Points.CompareTo(x.Points));

            // Display top 4 players and their points
            for (int i = 0; i < players.Count && i < 4; i++)
            {
                if (i == 0)
                {
                    NameTop1.Text = players[i].Name;
                    Point1.Text = players[i].Points.ToString();
                }
                else if (i == 1)
                {
                    NameTop2.Text = players[i].Name;
                    Point2.Text = players[i].Points.ToString();
                }
                else if (i == 2)
                {
                    NameTop3.Text = players[i].Name;
                    Point3.Text = players[i].Points.ToString();
                }
                else if (i == 3)
                {
                    NameTop4.Text = players[i].Name;
                    Point4.Text = players[i].Points.ToString();
                }
            }
        }

        private void FinalRanking_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance = null; // Reset instance on form close
        }
    }
}
