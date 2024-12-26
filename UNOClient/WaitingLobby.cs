using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UnoOnline
{
    public partial class WaitingLobby : Form
    {
        public WaitingLobby()
        {
            InitializeComponent();
            ClientSocket.OnMessageReceived += ClientSocket_OnMessageReceived;
            // Thêm người chơi vào list players
            GameManager.Instance.Players.Add(new Player(Program.player.Name));
            // Cập nhật danh sách người chơi
            UpdatePlayerList(GameManager.Instance.Players);
        }

        private void ClientSocket_OnMessageReceived(string message)
        {
            if (this.IsHandleCreated)
            {
                // Update the UI with the received message
                this.Invoke(new Action(() =>
                {
                    // Display the message in a label or text box
                    richTextBox1.Text = message;
                }));
            }
            else
            {
                // Handle the case where the handle has not been created
                this.HandleCreated += (s, e) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        // Display the message in a label or text box
                        richTextBox1.Text = message;
                    }));
                };
            }
        }

        private void btnJoinGame_Click(object sender, EventArgs e)
        {
            var message = new Message(MessageType.Start, new List<string> { Program.player.Name });
            ClientSocket.SendData(message);
        }

        private void WaitingLobby_FormClosing(object sender, FormClosingEventArgs e)
        {
          ClientSocket.SendData(new Message(MessageType.Leavegame, new List<string> { Program.player.Name }));
            this.Hide();
            Menu menu = new Menu();
            menu.Show();
        }
        public void UpdatePlayerList(List<Player> players)
        {
            //Show each player on each label
            for (int i = 0; i < players.Count; i++)
            {
                Label label = (Label)this.Controls.Find($"lblPlayer{i + 1}", true)[0];
                label.Text = players[i].Name;
            }
        }
    }
}