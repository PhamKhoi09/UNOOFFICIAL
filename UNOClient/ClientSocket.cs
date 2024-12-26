using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace UnoOnline
{
    public class ClientSocket
    {
        public static Socket clientSocket;
        public static Thread recvThread;
        public static readonly object lockObject = new object();
        public static GameManager gamemanager = GameManager.Instance;
        public static event Action<string> OnMessageReceived;
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // Hàm kết nối tới server
        public static void ConnectToServer(IPEndPoint server)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(server);
                //cancellationTokenSource = new CancellationTokenSource();
                recvThread = new Thread(() => ReceiveData(cancellationTokenSource.Token));
                recvThread.Start();

                if (gamemanager == null)
                {
                    gamemanager = GameManager.Instance;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to server: " + ex.Message);
            }
        }


        private static void ReceiveData(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    if (bytesRead > 0)
                    {
                        string messageString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        string keyText = "FhT4itYhSrEyj3Ki";
                        string keyIV = "xsDaePnNFJV9JrBw";
                        byte[] key = Encoding.UTF8.GetBytes(keyText);
                        byte[] iv = Encoding.UTF8.GetBytes(keyIV);
                        string Decryptmessage = DecryptAES(messageString, key, iv);
                        Message receivedMessage = Message.FromString(Decryptmessage);
                        AnalyzeData(receivedMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    MessageBox.Show("Error receiving data: " + ex.Message);
                }
            }
        }

        private static void AnalyzeData(Message message)
        {
            try
            {
                // Ensure GameManager instance is initialized
                if (gamemanager == null)
                {
                    gamemanager = GameManager.Instance;
                }

                switch (message.Type)
                {
                    case MessageType.LoginSuccessful:
                        OnMessageReceived.Invoke("Processing LoginSuccessful message");
                        Login.HandleLoginSuccessful(message.Data[0]);
                        break;
                    case MessageType.LoginFail:
                        OnMessageReceived.Invoke("Processing LoginFail message");
                        MessageBox.Show(message.Data[0], "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case MessageType.RegisterSuccessful:
                        OnMessageReceived.Invoke("Processing RegisterSuccessful message");
                        Register.HandleRegisterSuccessful();
                        break;
                    case MessageType.RegisterFail:
                        OnMessageReceived.Invoke("Processing RegisterFail message");
                        MessageBox.Show("Không thể đăng ký, vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case MessageType.GetpasswordSuccessful:
                        OnMessageReceived.Invoke("Processing GetpasswordSuccessful message");
                        GetPassword.HandleGetPasswordSuccessful(message.Data[2]);
                        break;
                    case MessageType.GetpasswordFail:
                        OnMessageReceived.Invoke("Processing GetpasswordFail message");
                        MessageBox.Show(message.Data[0], "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case MessageType.Info:
                        OnMessageReceived?.Invoke(message.Data[0]);
                        if (gamemanager != null)
                        {
                            gamemanager.UpdateOtherPlayerName(message.Data[0]);
                        }
                        break;
                    case MessageType.InitializeStat:
                        OnMessageReceived?.Invoke(string.Join(" ", message.Data));
                        GameManager.InitializeStat(message);
                        break;
                    case MessageType.OtherPlayerStat:
                        OnMessageReceived?.Invoke("Processing OtherPlayerStat message");
                        GameManager.UpdateOtherPlayerStat(message);
                        break;
                    case MessageType.Boot:
                        OnMessageReceived?.Invoke("Processing Boot message");
                        if (Application.OpenForms[0].InvokeRequired)
                        {
                            Application.OpenForms[0].Invoke(new Action(() => GameManager.Boot()));
                        }
                        else
                        {
                            GameManager.Boot();
                        }
                        break;
                    case MessageType.Update:
                        OnMessageReceived?.Invoke("Processing Update");
                        gamemanager.HandleUpdate(message);
                        break;
                    case MessageType.Turn:
                        OnMessageReceived?.Invoke("Processing Turn message");
                        GameManager.HandleTurnMessage(message);
                        break;
                    case MessageType.CardDraw:
                        GameManager.HandleCardDraw(message);
                        break;
                    case MessageType.Specialdraws:
                        OnMessageReceived?.Invoke(string.Join(" ", message.Data));
                        GameManager.HandleSpecialDraw(message);
                        break;
                    case MessageType.ChatMessage:
                        GameManager.HandleChatMessage(message);
                        break;
                    case MessageType.Penalty:
                        GameManager.Penalty(message);
                        break;
                    case MessageType.YellUNOEnable:
                        Form1.YellUNOEnable();
                        break;
                    case MessageType.End:
                        GameManager.HandleEndMessage(message);
                        break;
                    case MessageType.Result:
                        GameManager.HandleResult(message);
                        break;
                    case MessageType.NotEnoughPlayers:
                        GameManager.HandleNotEnoughPlayer(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                if (Application.OpenForms[0].InvokeRequired)
                {
                    Application.OpenForms[0].Invoke(new Action(() => MessageBox.Show($"Error analyzing data: {ex.Message}\n{ex.StackTrace}")));
                }
                else
                {
                    MessageBox.Show($"Error analyzing data: {ex.Message}\n{ex.StackTrace}");
                }
            }

        }

        public static void SendData(Message message)
        {
            try
            {
                string messageString = message.ToString();
                string keyText = "FhT4itYhSrEyj3Ki";
                string keyIV = "xsDaePnNFJV9JrBw";
                byte[] key = Encoding.UTF8.GetBytes(keyText);
                byte[] iv = Encoding.UTF8.GetBytes(keyIV);
                string encryptmessage = EncryptAES(messageString, key, iv);
                byte[] buffer = Encoding.UTF8.GetBytes(encryptmessage);
                clientSocket.Send(buffer);
            }
            catch 
            {
                MessageBox.Show("Xin lỗi bạn, server đang bị lỗi, chúng tôi sẽ khắc phục trong thời gian nhanh nhất");
                //Tắt app
            }
        }

        public static void Disconnect()
        {
            try
            {
                cancellationTokenSource.Cancel();

                if (clientSocket != null && clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }

                if (recvThread != null && recvThread.IsAlive)
                {
                    recvThread.Join();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during disconnect: " + ex.Message);
            }
        }

        private void HandleReceive(IAsyncResult ar)
        {
            try
            {
                int received = clientSocket.EndReceive(ar);
                if (received > 0)
                {
                    byte[] buffer = (byte[])ar.AsyncState;
                    string message = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Received from server: {message}"); // Debug log

                    OnMessageReceived?.Invoke(message);
                }

                StartReceive(); // Tiếp tục nhận dữ liệu
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReceive: {ex.Message}");
            }
        }

        private void StartReceive()
        {
            try
            {
                byte[] buffer = new byte[1024];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(HandleReceive), buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StartReceive: {ex.Message}");
            }
        }

        private static string EncryptAES(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                // Thiết lập khóa và IV cho AES
                aes.Key = key;
                aes.IV = iv;
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                // Tạo đối tượng mã hóa
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        private static string DecryptAES(string encryptedText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                // Tạo đối tượng giải mã
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
public enum MessageType
{
    Login,
    LoginSuccessful,
    LoginFail,
    Register,
    RegisterSuccessful,
    RegisterFail,
    Getpassword,
    GetpasswordSuccessful,
    GetpasswordFail,

    Joingame,
    Leavegame,
    Start,
    RutBai,
    YellUNO,
    Penalty,
    ChatMessage,
    Chat,
    DanhBai,
    DrawPenalty,
    Info,
    InitializeStat,
    OtherPlayerStat,
    Boot,
    Update,
    Turn,
    CardDraw,
    Specialdraws,
    SpecialCardEffect,
    YellUNOEnable,
    NotEnoughPlayers,
    End,
    Result,
    Diem,
    
    Restart,
    Finish,
    Disconnect,
}
public class Message
{
    public MessageType Type { get; set; }
    public List<string> Data { get; set; }

    public Message()
    {
        Data = new List<string>();
    }

    public Message(MessageType type, List<string> data)
    {
        Type = type;
        Data = data;
    }

    public override string ToString()
    {
        return $"{Type};{string.Join(";", Data)}";
    }

    public static Message FromString(string messageString)
    {
        try
        {
            var parts = messageString.Split(new[] { ';' }, 2);
            if (parts.Length < 2)
            {
                throw new FormatException("Invalid message format");
            }

            var type = (MessageType)Enum.Parse(typeof(MessageType), parts[0]);
            var data = parts[1].Split(';').ToList();
            return new Message(type, data);
        }
        catch (Exception ex)
        {
            throw new FormatException("Invalid message format", ex);
        }
    }
}
