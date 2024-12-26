using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnoOnline
{
    public partial class Login : Form
    {
        

        public Login()
        {
            InitializeComponent();
            ClientSocket.OnMessageReceived += OnMessageReceived;
        }
        private void OnMessageReceived(string message)
        {
            if (this.IsHandleCreated)
            {
                // Update the UI with the received message
                this.Invoke(new Action(() =>
                {
                    // Display the message in a label or text box
                    textBox1.Text = message;
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
                        textBox1.Text = message;
                    }));
                };
            }
        }
        public  void Button_DangNhap_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ TextBox
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên tài khoản và mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var message = new Message(MessageType.Login, new List<string> { username, password });
            ClientSocket.SendData(message);
        }

        public static void HandleLoginSuccessful(string username)
        {
            MessageBox.Show("Đăng nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Program.player.Name = username;
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login != null)
            {
                login.Invoke(new Action(() =>
                {
                    login.Hide();
                }));
            }
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                if (!Program.IsFormOpen(typeof(Menu)))
                {
                    Menu menu = new Menu();
                    menu.Show();
                }
            }));
        }
        private void button_DangKy_Click(object sender, EventArgs e)
        {
            // Mở form đăng ký
            Register registerForm = new Register();
            registerForm.ShowDialog();
        }

        private void ForgotPasswordButton_Click_Click(object sender, EventArgs e)
        {
            // Mở form lấy lại mật khẩu
            GetPassword forgotPasswordForm = new GetPassword();
            forgotPasswordForm.ShowDialog();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string serverIP = tbServerIP.Text.Trim();
            if (string.IsNullOrEmpty(serverIP))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ IP của server.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), 11000);
            ClientSocket.ConnectToServer(serverEP);
        }
    }
}
