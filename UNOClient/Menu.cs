using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnoOnline
{
    public partial class Menu : Form
    {
        public List<Player> players = new List<Player>();

        public Menu()
        {
            InitializeComponent();
        }

        private void BtnJoinGame_Click(object sender, EventArgs e)
        {
            var message = new Message(MessageType.Joingame, new List<string> { Program.player.Name });
            ClientSocket.SendData(message);
            WaitingLobby waitingLobby = new WaitingLobby();
            waitingLobby.Show();
            this.Hide();
        }

        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                var message = new Message(MessageType.Disconnect, new List<string> { Program.player.Name });
                ClientSocket.SendData(message);
                ClientSocket.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during disconnect: {ex.Message}");
            }
            Environment.Exit(0);
            Application.Exit();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            try
            {
                var message = new Message(MessageType.Disconnect, new List<string> { Program.player.Name });
                ClientSocket.SendData(message);
                ClientSocket.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during disconnect: {ex.Message}");
            }
            Environment.Exit(0);
            Application.Exit();
        }

        private void BtnRules_Click(object sender, EventArgs e)
        {
            //Sẽ tạm thời mở trang web chứa luật chơi-https://www.unorules.com/
            System.Diagnostics.Process.Start("https://www.unorules.com/");
        }
    }
}