using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnoOnline
{
    public partial class GetPassword : Form
    {
        public GetPassword()
        {
            InitializeComponent();
        }

        private void retrievePasswordButton_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ TextBox
            string username = usernameTextBox.Text.Trim();
            string email = emailTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên tài khoản và email.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ClientSocket.SendData(new Message(MessageType.Getpassword, new List<string> { username, email }));
        }

        public static void HandleGetPasswordSuccessful(string password)
        {
            TextBox passwordTextBox = (TextBox)Application.OpenForms["GetPassword"].Controls["passwordTextBox"];
            passwordTextBox.Text = password;
        }
    }
}
