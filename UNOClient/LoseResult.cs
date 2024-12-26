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
    public partial class LoseResult : Form
    {
        public LoseResult()
        {
            InitializeComponent();
        }
        private void btnRestart_Click(object sender, EventArgs e)
        {
            ClientSocket.SendData(new Message(MessageType.Restart, new List<string> { Program.player.Name }));
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            ClientSocket.SendData(new Message(MessageType.Finish, new List<string> { Program.player.Name }));
        }
    }
}
