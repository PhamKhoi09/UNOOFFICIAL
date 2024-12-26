using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Linq.Expressions;

namespace UNOServer
{

    class Program
    {
        private static Socket ServerSocket; //Socket cho server (socket server)
        private static Socket ClientSocket; //Socket cho client (socket client)
        private static Thread ClientThread; //Thread để xử lý kết nối từ client khác
        private static List<PLAYER> PLAYERLIST0 = new List<PLAYER>(); //List các người chơi kết nối đến server với các thông tin của người chơi từ class PLAYER
        private static List<PLAYER> PLAYERLIST = new List<PLAYER>(); //List các người chơi đang chơi với các thông tin của người chơi từ class PLAYER
        private static int HienTai = 1; //Đến lượt đánh của người chơi nào
        private static bool ChieuDanh = true; //Chiều đánh
        private static int RUT = 0; //Số bài rút (cho lá df, dt)
        private static List<string> YELLUNOLIST = new List<string>(); //List các id chỉ còn 1 lá mà chưa hô uno
        private static int DemRestart = 0; //Đếm số lượng đồng ý restart (màn hình kết quả thắng thua)
        private static int DemFinish = 0; //Đếm số lượng muốn finish (màn hình kết quả thắng thua)
        private static string WinnerName = ""; //Lưu tên người thắng
        private static bool TrangThai = false; //Trạng thái game: chưa bắt đầu/đã kết thúc ván game hoặc kết thúc hẳn (false), đang diễn ra (true)
        
        /* Hàm thiết lập (khởi động) server */
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //Sử dụng console để cập nhật thông tin (tiện theo dõi bên server)
            IPHostEntry Host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress IPaddress = Host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Tạo socket cho server
            IPEndPoint ServerEP = new IPEndPoint(IPaddress, 11000); //Tạo endpoint với IP của host và cổng
            ServerSocket.Bind(ServerEP); //Socket server kết nối đến endpoint đó => địa chỉ của server
            ServerSocket.Listen(4);
            Console.WriteLine("Server đã được tạo và đang chạy! Đợi các kết nối từ Clients...");
            Console.WriteLine("Địa chỉ IP của server: " + IPaddress.ToString());
            //Lặp vô hạn để xử lý các kết nối đến server từ nhiều client
            while (true)
            {
                //Nếu server chưa full (4 người) thì cho phép thiết lập kết nối
                if (PLAYERLIST.Count < 4)
                {
                    ClientSocket = ServerSocket.Accept(); //Server chấp nhận kết nối từ 1 client nào đó và tạo socket client tương ứng
                    Console.WriteLine("Nhận kết nối từ " + ClientSocket.RemoteEndPoint);
                    ClientThread = new Thread(() => HandleNewPlayer(ClientSocket)); //Tạo thread mới để chạy hàm HandleNewPlayer để xử lý cho socket client tương ứng 
                    ClientThread.Start();
                }
                else //Nếu server đã full, tạo socket tạm thời để gửi thông báo đã đầy rồi đóng
                {                   
                    Socket TempSocket = ServerSocket.Accept(); //Tạo socket tạm thời để chấp nhận
                    string note = "Server đã đầy.";
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(note));
                    TempSocket.Send(data); //Gửi thông báo cho client
                    Console.WriteLine("Số lượng người chơi đã đạt tối đa (4)!");
                    TempSocket.Shutdown(SocketShutdown.Both);
                    TempSocket.Close(); //Đóng socket tạm thời
                }
            }
        }

        /* Hàm quản lý từng kết nối client và xử lý yêu cầu từng người chơi khác nhau */
        public static void HandleNewPlayer(Socket client)
        {
            PLAYER User = new PLAYER(); //Tạo đối tượng người chơi
            User.PlayerSocket = client; //Thông tin socket client được gán cho socket người chơi (PlayerSocket) là thuộc tính socket của người chơi trong class PLAYER 
            PLAYERLIST0.Add(User); //Thêm người chơi đó vào list người chơi kết nối đến server
            byte[] data = new byte[1024]; //Tạo mảng byte tên data để chứa dữ liệu nhận được từ client
            //Vòng lặp kiểm tra kết nối và xử lý dữ liêu từ client 
            while (User.PlayerSocket.Connected)
            {
                try
                { 
                    if (User.PlayerSocket.Available > 0) //Nếu có dữ liệu đến từ client thì server sẽ bắt đầu nhận
                    {
                        string receivedata = ""; //Tạo chuỗi chứa thông điệp (dữ liệu từ client gửi đến)
                        string Decryptmessage = ""; //Tạo chuỗi chứa thông điệp đã giải mã
                        while (User.PlayerSocket.Available > 0)
                        {
                            int read = User.PlayerSocket.Receive(data); //Nhận dữ liệu client và ghi từng byte dữ liệu vào mảng byte tên data, số byte lưu vào read
                            receivedata += Encoding.UTF8.GetString(data, 0, read); //Chuyển đổi mảng byte dữ liệu thành dạng chuỗi và nối chuỗi vào receivedata thành thông điệp
                            string keyText = "FhT4itYhSrEyj3Ki";
                            string keyIV = "xsDaePnNFJV9JrBw";
                            byte[] key = Encoding.UTF8.GetBytes(keyText);
                            byte[] iv = Encoding.UTF8.GetBytes(keyIV);
                            Decryptmessage = DecryptAES(receivedata, key, iv); //Giải mã tin nhận được từ client
                        }
                        Console.WriteLine(User.PlayerSocket.RemoteEndPoint + ": " + receivedata); //Thông điệp chưa giải mã
                        Console.WriteLine(User.PlayerSocket.RemoteEndPoint + ": " + Decryptmessage); //Thông điệp đã giải mã
                        AnalyzingMessage(Decryptmessage, User); //Thông điệp được đưa vào hàm này để xử lý yêu cầu (thông điệp) từ người chơi (client) tương ứng
                    }
                }
                catch (Exception) //Xử lý trường hợp thoát đột ngột bên client
                {
                    if (TrangThai == true && PLAYERLIST.Count == 2)
                    {
                        string ID = User.ID;
                        foreach (var user in PLAYERLIST.ToList()) //Duyệt qua các người chơi trong PLAYERLIST
                        {
                            if (user.ID != ID) //Nếu ID trùng với ID của người chơi không là người thoát
                            {
                                string SendData = "NotEnoughPlayers;" + user.ID;
                                byte[] Data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                                user.PlayerSocket.Send(Data);
                                Console.WriteLine("Quá ít người chơi, không thể bắt đầu");
                            }
                            PLAYERLIST.Remove(user);
                        }
                        //Đóng kết nối với người thoát đột ngột
                        User.PlayerSocket.Shutdown(SocketShutdown.Both);
                        User.PlayerSocket.Close();
                        Console.WriteLine(User.ID + " đã thoát đột ngột!");
                        PLAYERLIST.Remove(User);
                        PLAYERLIST0.Remove(User);
                        TrangThai = false;
                        Console.WriteLine("Trạng thái game bây giờ " + TrangThai);
                    }
                    else
                    {
                        try
                        {
                            User.PlayerSocket.Shutdown(SocketShutdown.Both);
                            User.PlayerSocket.Close();
                            if (TrangThai == true && PLAYERLIST[HienTai - 1].ID == User.ID) //Nếu game đang diễn ra và người đang đến lượt lại thoát, update lượt cho người khác
                            {
                                if (ChieuDanh == true)
                                    HienTai--;
                                Console.WriteLine(User.ID + " đã thoát đột ngột!");
                                PLAYERLIST.Remove(User); //Xóa người chơi khỏi danh sách PLAYERLIST
                                PLAYERLIST0.Remove(User);
                                UpdateTurn();
                            }
                            else if (TrangThai == true) //Nếu game đang diễn ra và người đang không đến lượt lại thoát, update lượt cho người khác
                            {
                                if (User.Luot < PLAYERLIST[HienTai - 1].Luot)
                                    HienTai--;
                                Console.WriteLine(User.ID + " đã thoát đột ngột!");
                                PLAYERLIST.Remove(User); //Xóa người chơi khỏi danh sách PLAYERLIST
                                PLAYERLIST0.Remove(User);
                                UpdateTurn();
                            }
                            else
                            {
                                if (User.ID == "")
                                    Console.WriteLine(User.PlayerSocket.RemoteEndPoint + " đã thoát đột ngột!");
                                else
                                    Console.WriteLine(User.ID + " đã thoát đột ngột!");
                                PLAYERLIST0.Remove(User); //Xóa người chơi khỏi danh sách PLAYERLIST0 các trường hợp còn lại
                            }
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            }
        }

        /* Hàm xử lý yêu cầu (thông điệp) của người chơi (client) tương ứng */
        public static void AnalyzingMessage(string receivedata, PLAYER User)
        {
            //Tạo mảng chuỗi Message với mỗi phần tử chứa từng phần trong tham số chuỗi receivedata chứa thông điệp
            //Mỗi phần trong thông điệp được phân biệt bởi dấu ; và được lưu lần lượt vào từng phần tử
            //Ví dụ thông điệp là "Joingame;User1;..." thì lưu vào mảng sẽ là ["Joingame", "User1", ...]
            string[] Message = receivedata.Split(';');
            switch (Message[0]) //Xét phần tử đầu tiên trong mảng Message chứa loại thông điệp (phần đầu tiên trong thông điệp) được gửi từ client
            {
                case "Login":
                    HandleLogin(Message, User);
                    break;
                case "Register":
                    HandleRegister(Message, User);
                    break;
                case "Getpassword":
                    HandleGetpassword(Message, User);
                    break;
                case "Joingame":
                    HandleJoingame(Message, User);
                    break;
                case "Leavegame":
                    HandleLeavegame(Message);
                    break;
                case "Start":
                    SetupGame(Message, User);
                    break;
                case "DanhBai":
                    HandleDanhBai(Message, User);
                    break;
                case "RutBai":
                    HandleRutBai(Message, User);
                    break;
                case "SpecialCardEffect":
                    HandleSpecialDraw(Message, User);
                    break;
                case "YellUNO":
                    YELLUNOLIST.Remove(Message[1]);
                    break;
                case "DrawPenalty":
                    HandleSpecialDraw(Message, User);
                    break;
                case "Diem":
                    HandleDiem(Message, User);
                    break;
                case "Chat":
                    HandleChat(Message, User);
                    break;
                case "Restart":
                    HandleAfterMatch(Message, User);
                    break;
                case "Finish":
                    HandleAfterMatch(Message, User);
                    break;
                case "Disconnect":
                    HandleDisconnect(Message, User);
                    break;
                default:
                    break;
            }
        }

        /*                                                          Cấu trúc thông điệp giữa Server và Client
         *                  Client -> Server                                          |                                     Server -> Client
         * Login;ID;<Password>                                                        | LoginSuccessful/LoginFail;ID/<Thông báo lỗi nếu lỗi>
         * Register;ID;<email>;<password>                                             | RegisterSuccessful/RegisterFail;ID/<Thông báo lỗi nếu lỗi>                   
         * Getpassword;ID;<email>                                                     | GetpasswordSuccessful/GetpasswordFail;ID/<Thông báo lỗi nếu lỗi>;<password> (nếu thành công)
         * Joingame;ID                                                                | Info;ID         
         * Leavegame;ID                                                               | InitializeStat;ID;Luot;SoLuongBai;CardName;CardName...;CardName (7 bài người chơi + 1 bài hệ thống tự đánh)              
         * Start;ID                                                                   | OtherPlayerStat;ID;Luot;SoLuongBai
         * DanhBai;ID;SoLuongBai;CardName;color(wild draw, wild)                      | Boot;ID                                   
         * RutBai;ID;SoLuongBai                                                       | Update;ID;SoluongBai;CardName(Nếu đánh bài);color(wild draw, wild) (Nếu đánh bài)          
         * SpecialCardEffect;ID;SoLuongBai;                                           | Turn;ID                      
         * Chat;ID;<Content>                                                          | CardDraw;ID;CardName                
         * YellUNO;ID                                                                 | Specialdraws;ID;CardName;CardName...
         * DrawPenalty;ID;SoLuongBai;                                                 | End;ID
         * Diem;ID;<Diem so>                                                          | ChatMessage;ID;<Content>
         * Restart;ID                                                                 | YellUNOEnable;ID
         * Finish;ID                                                                  | Penalty;ID
         * Disconnect;ID                                                              | Result;ID;Diem;Rank
         *                                                                            | NotEnoughPlayers;ID
         * LƯU Ý: 
         * Bên client sẽ tự động disable nút hô UNO sau khi người chơi ấn nút hô UNO hoặc khi lại đến lượt người chơi đó quên ấn.
         * Bên client sẽ xử lý logic việc show những lá bài có thể đánh hoặc không trong bộ bài dựa trên thông điệp Update lá bài face up card bên server gửi đến (cùng màu, cùng số, đặc biệt trường hợp face up card là lá df, wd phải dựa trên màu người chơi đánh lá đó đã chọn).
         * Về restart/finish game: bên client sau khi hiện màn hình thắng thua thì có 2 nút restart/finish và gửi thông điệp tương ứng, server khi nào nhận đủ thông điệp từ tất cả người chơi thì mới xử lý và quyết định restart hay hiện màn hình xếp hạng kết thúc.
         * Client ấn nút restart/finish xong thì disable tất cả các nút đi tránh gửi nhiều lần. 
        */
        /* Hàm xử lý việc đăng nhập của người chơi */
        private static void HandleLogin(string[] Message,PLAYER User)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\C++\NEWUNOCLONE\UNOServer\UNOServer\UserDatabase.mdf;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Truy vấn lấy mật khẩu đã lưu từ bảng TaiKhoan
                string query = "SELECT MatKhau FROM TaiKhoan WHERE TenTaiKhoan = @Username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", Message[1]);
                    // Lấy mật khẩu đã lưu
                    string storedPassword = cmd.ExecuteScalar()?.ToString();
                    if (storedPassword != null && PasswordHelper.VerifyPassword(Message[2], storedPassword))
                    {
                        string SendData = "LoginSuccessful;" + Message[1];
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        User.PlayerSocket.Send(data);
                        Console.WriteLine("Note: " + SendData);
                    }
                    else
                    {
                        string SendData = "LoginFail;Sai tên hoặc mật khẩu!";
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        User.PlayerSocket.Send(data);
                        Console.WriteLine("Note: " + SendData);
                    }
                }
            }
        }

        /* Hàm xử lý việc đăng ký tài khoản cho người dùng */
        private static void HandleRegister(string[] Message, PLAYER User)
        {
            string passwordHash = PasswordHelper.HashPassword(Message[3]);
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\C++\NEWUNOCLONE\UNOServer\UNOServer\UserDatabase.mdf;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Kiểm tra xem tài khoản đã tồn tại hay chưa
                string checkQuery = "SELECT COUNT(*) FROM TaiKhoan WHERE TenTaiKhoan = @Username";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Username", Message[1]);
                    int userCount = (int)checkCmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        string SendData = "RegisterFail;Tên tài khoản đã tồn tại!";
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        User.PlayerSocket.Send(data);
                        Console.WriteLine("Note: " + SendData);
                    }
                }
                // Thêm dữ liệu vào bảng TaiKhoan
                string query = "INSERT INTO TaiKhoan (TenTaiKhoan, MatKhau, Email) VALUES (@Username, @PasswordHash, @Email)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", Message[1]);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@Email", Message[2]);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        string SendData = "RegisterSuccessful;" + Message[1];
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        User.PlayerSocket.Send(data);
                        Console.WriteLine("Note: " + SendData);
                    }
                }
            }
        }

        /* Hàm xử lý lấy mật khẩu mới cho người dùng quên (phải đúng email) */
        private static void HandleGetpassword(string[] Message, PLAYER User)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\C++\NEWUNOCLONE\UNOServer\UNOServer\UserDatabase.mdf;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Truy vấn kiểm tra email dựa trên tên tài khoản
                string query = "SELECT Email FROM TaiKhoan WHERE TenTaiKhoan = @Username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", Message[1]);
                    string storedEmail = cmd.ExecuteScalar()?.ToString();
                    if (storedEmail != null && storedEmail.Equals(Message[2], StringComparison.OrdinalIgnoreCase))
                    {
                        // Tạo mật khẩu mới
                        string newPassword = GenerateRandomPassword();
                        // Băm mật khẩu mới
                        string hashedPassword = PasswordHelper.HashPassword(newPassword);
                        // Cập nhật mật khẩu vào database
                        UpdatePasswordInDatabase(Message[1], hashedPassword);
                        string SendData = "GetpasswordSuccessful;" + newPassword;
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        User.PlayerSocket.Send(data);
                        Console.WriteLine("Note: " + SendData);
                    }
                    else
                    {
                        string SendData = "GetpasswordFail;Sai email của tài khoản này!";
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        User.PlayerSocket.Send(data);
                        Console.WriteLine("Note: " + SendData);
                    }
                }
            }
        }

        /* Hàm tạo password ngẫu nhiên */
        private static string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8); // Mật khẩu ngẫu nhiên 8 ký tự
        }

        // Hàm cập nhật mật khẩu mới vào database
        private static void UpdatePasswordInDatabase(string username, string hashedPassword)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\C++\NEWUNOCLONE\UNOClient\Database1.mdf;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE TaiKhoan SET MatKhau = @PasswordHash WHERE TenTaiKhoan = @Username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /* Hàm mã hóa tin */
        private static string Encrypt(string message)
        {
            string keyText = "FhT4itYhSrEyj3Ki";
            string keyIV = "xsDaePnNFJV9JrBw";
            byte[] key = Encoding.UTF8.GetBytes(keyText);
            byte[] iv = Encoding.UTF8.GetBytes(keyIV);
            string encryptmessage = EncryptAES(message, key, iv);
            return encryptmessage;
        }

        /* Hàm mã hóa bằng thuật toán AES */
        private static string EncryptAES(string PlainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                // Thiết lập khóa và IV cho AES
                aes.Key = key;
                aes.IV = iv;
                byte[] inputBytes = Encoding.UTF8.GetBytes(PlainText);
                // Tạo đối tượng mã hóa
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        /* Hàm giải mã bằng thuật toán AES */
        private static string DecryptAES(string EncryptedText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                byte[] encryptedBytes = Convert.FromBase64String(EncryptedText);
                // Tạo đối tượng giải mã
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
        /* Hàm khởi tạo lượt và gán số bài ban đầu cho người chơi */
        public static void SetupTurn()
        {
            int[] Turn = new int[PLAYERLIST.Count]; //Tạo mảng int tên Turn với kích thước là số lượng người chơi kết nối đến server trong list 
            for (int i = 1; i <= PLAYERLIST.Count; i++)
            {
                Turn[i - 1] = i; //Gán vào từng phần tử mảng Turn các số từ 1 đến số lượng người chơi trong list để lưu thứ tự chơi
            }
            Random random = new Random(); //Tạo đối tượng random thuộc lớp Random 
            //Trộn thứ tự chơi ngẫu nhiên cho các người chơi
            foreach (var user in PLAYERLIST)
            {
                int pick = random.Next(Turn.Length); //Tạo biến int tên pick để lưu thứ tự chơi được random random từ mảng Turn
                user.Luot = Turn[pick]; //Gán thứ tự chơi đó cho thuộc tính Luot của người chơi trong class PLAYER
                Turn = Turn.Where(val => val != Turn[pick]).ToArray(); //Xóa thứ tự chơi đã được gán ra khỏi mảng Turn để các người chơi tiếp theo không bị chọn trùng thứ tự
                user.SoLuongBai = 7; //Gán số lượng bài lúc bắt đầu game người chơi là 7
            }
        }

        /* Hàm xào bộ bài */
        public static void ShuffleCards()
        {
            Random random = new Random(); //Tạo đối tượng random thuộc lớp Random
            BOBAI.CardName = BOBAI.CardName.OrderBy(x => random.Next()).ToArray(); //Sắp xếp tên các lá bài trong mảng CardName 1 cách ngẫu nhiên do random random là thuộc tính của lớp BOBAI
        }

        /* Hàm tạo bài ban đầu cho người chơi */
        public static string CreatePlayerCards()
        {
            Random random = new Random(); //Tạo đối tượng random thuộc lớp Random
            string playercards = ""; //Tạo chuỗi playercards
            //Lấy 7 lá bài
            for (int i = 0; i < 7; i++)
            {
                int pick = random.Next(BOBAI.CardName.Length); //Tạo biến int tên pick để lưu chỉ số trong mảng CardName ngẫu nhiên do random random
                playercards += BOBAI.CardName[pick] + ";"; //Thêm lá bài vào chuỗi playercards
                BOBAI.CardName = BOBAI.CardName.Where(val => val != BOBAI.CardName[pick]).ToArray(); //Xóa lá bài đã được chọn ra khỏi mảng CardName để các lá sau không bị chọn trùng
            }
            return playercards; //Trả về chuỗi playercards chứa 7 lá bài được ghép lại với nhau, mỗi lá cách nhau bởi dấu chấm phẩy ;
        }

        /* Hàm hệ thống tự đánh (mở) lá bài đầu tiên */
        public static string ShowFirstCard()
        {
            string temp = ""; //Tạo chuỗi temp để lưu lá bài 
            //Duyệt qua tất cả các lá bài chỉ lựa các lá bài số để đánh đầu tiên
            for (int i = 0; i < BOBAI.CardName.Length; i++)
            {
                temp = BOBAI.CardName[i]; //Lấy lá bài của mảng CardName và lưu vào chuỗi temp
                //Nếu thỏa điều kiện chỉ là lá bài số thì break khỏi vòng lặp
                if (!temp.Contains("Reverse") && !temp.Contains("Skip") && !temp.Contains("Wild") && !temp.Contains("Wild_Draw") && !temp.Contains("Draw")) //Sử dụng Contains() để xác định trong lá bài có phần cần tìm hay không
                //Lưu ý là contains() sử dụng với chuỗi không yêu cầu chuỗi gốc phải khớp hoàn toàn nên đáng lý ra là chỉ cần contains Wild và Draw nhưng t trình bày hết cho dễ hiểu
                    break;
            }
            BOBAI.CardName = BOBAI.CardName.Where(val => val != temp).ToArray(); //Xóa lá bài đã được chọn ra khỏi mảng CardName để không bị sử dụng lại
            return temp; //Trả về chuỗi temp chứa lá bài đã lật
        }

        /* Hàm gửi thông tin của tất cả người chơi đã kết nối cho người chơi mới và ngược lại */
        private static void HandleJoingame(string[] Message, PLAYER User)
        {
            PLAYERLIST.Add(User);
            User.ID = Message[1]; // Thiết lập ID (danh tính) của người chơi từ dữ liệu đã nhận
            //Gửi thông tin của những người chơi khác đến người chơi mới
            foreach (var user in PLAYERLIST)
            {
                string SendData = "Info;" + user.ID;
                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData)); //Tạo mảng byte tên data để chứa thông điệp theo cấu trúc
                User.PlayerSocket.Send(data); // Gửi data chứa thông điệp mang ID của mỗi người chơi trong PLAYERLIST đến người chơi mới
                Thread.Sleep(210);
            }
            //Gửi thông tin của người chơi mới đến những người chơi khác
            foreach (var user in PLAYERLIST)
            {
                if (user.PlayerSocket != User.PlayerSocket)
                {
                    string SendData = "Info;" + User.ID;
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData)); //Tạo mảng byte tên data để chứa thông điệp theo cấu trúc
                    user.PlayerSocket.Send(data); // Gửi data chứa thông điệp mang ID của người chơi mới đến các người chơi khác
                    Thread.Sleep(210);
                }
            }
        }

        /* Hàm xử lý yêu cầu rời từ một người chơi */
        private static void HandleLeavegame(string[] Message)
        {
            //Nếu chỉ có hoặc còn 2 người chơi trong game đang diễn ra mà lại có người leave thì đóng kết nối với người còn lại luôn
            if (TrangThai == true && PLAYERLIST.Count == 2)
            {
                foreach (var user in PLAYERLIST.ToList()) //Duyệt qua các người chơi trong PLAYERLIST
                {
                    if (user.ID != Message[1]) //Nếu ID trùng với ID của người chơi không là người muốn ngắt kết nối
                    {
                        string SendData = "NotEnoughPlayers;" + Message[1];
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        user.PlayerSocket.Send(data);
                        Console.WriteLine("Quá ít người chơi, không thể bắt đầu");
                    }
                    PLAYERLIST.Remove(user);
                }
                TrangThai = false;
                Console.WriteLine("Trạng thái game bây giờ " + TrangThai);
            }
            else
            {
                foreach (var user in PLAYERLIST.ToList()) //Duyệt qua các người chơi trong PLAYERLIST
                {
                    if (user.ID == Message[1]) //Nếu ID trùng với ID của người chơi muốn ngắt kết nối
                    {                  
                        if (TrangThai == true && PLAYERLIST[HienTai - 1].ID == user.ID) //Nếu game đang diễn ra và người đang đến lượt lại leave, update lượt cho người khác
                        {
                            if (ChieuDanh == true)
                                HienTai--;
                            PLAYERLIST.Remove(user); //Xóa người chơi khỏi danh sách PLAYERLIST
                            UpdateTurn();
                        }
                        else if (TrangThai == true) //Nếu game đang diễn ra và người đang không đến lượt lại leave, update lượt cho người khác
                        {
                            if (user.Luot < PLAYERLIST[HienTai - 1].Luot)
                                HienTai--;
                            PLAYERLIST.Remove(user); //Xóa người chơi khỏi danh sách PLAYERLIST
                            UpdateTurn();
                        }
                        else PLAYERLIST.Remove(user); //Xóa người chơi khỏi danh sách PLAYERLIST các trường hợp còn lại
                    }
                }
            }
        }

        /* Hàm thiết lập bắt đầu trò chơi */
        private static void SetupGame(string[] Message, PLAYER User)
        {
            //Nếu bắt đầu game khi không đủ 2 người trở lên, gửi thông điệp ko thể bắt đầu game
            if (PLAYERLIST.Count < 2)
            {
                string SendData = "NotEnoughPlayers;" + Message[1];
                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                User.PlayerSocket.Send(data);
                Console.WriteLine("Quá ít người chơi, không thể bắt đầu");
                return;
            }    
            TrangThai = true;
            Console.WriteLine("Trạng thái game bây giờ " + TrangThai);
            BOBAI.ResetCardName(); //Tạo mảng CardName mới
            SetupTurn(); //Tạo lượt và gán số bài 7 bài cho mỗi người chơi
            PLAYERLIST.Sort((x, y) => x.Luot.CompareTo(y.Luot)); //Sắp xếp lại các người chơi trong PLAYERLIST theo lượt tăng dần
            ShuffleCards(); //Xào bộ bài
            BOBAI.Current = ShowFirstCard(); //Tự động rút lá bài đầu tiên và cập nhật lá bài hiện tại đã đánh
            //Gửi thông điệp cho tất cả người chơi InitializeStat: Gửi thông điệp thông tin khởi tạo về danh tính, thứ tự lượt, số bài, tên các lá cụ thể cho mỗi người chơi lúc ban đầu 
            foreach (var user in PLAYERLIST)
            {
                string SendData = "InitializeStat;" + user.ID + ";" + user.Luot + ";" + user.SoLuongBai + ";" + CreatePlayerCards() + BOBAI.Current;
                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                user.PlayerSocket.Send(data);
                Thread.Sleep(200);
            }
            //Gửi thông điệp OtherPlayerStat: Gửi thông điệp chứa thông tin khởi tạo về danh tính, thứ tự lượt, số bài, những người chơi khác cho mỗi người chơi 
            //Ví dụ t là người chơi thì OtherPlayerStat này sẽ gửi thông tin những người chơi còn lại cho bên t để game cập nhật giao diên...và mỗi người chơi khác cũng thế
            foreach (var user in PLAYERLIST)
            {
                foreach (var player in PLAYERLIST)
                {
                    if (user.ID != player.ID)
                    {
                        string SendData = "OtherPlayerStat;" + player.ID + ";" + player.Luot + ";" + player.SoLuongBai;
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        user.PlayerSocket.Send(data);
                        Thread.Sleep(200);
                    }
                }
            }
            //Gửi thông điệp cho tất cả người chơi Boot: Gửi thông điệp yêu cầu mở màn hình game
            foreach (var user in PLAYERLIST)
            {
                string SendData = "Boot;" + user.ID;
                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                user.PlayerSocket.Send(data);
                Thread.Sleep(200);
            }

            //Gửi thông điệp cho tất cả người chơi Turn: Gửi thông điệp về việc đến lượt của người chơi nào (bắt đầu game)
            foreach (var user in PLAYERLIST)
            {
                string SendData = "Turn;" + PLAYERLIST[HienTai - 1].ID;
                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                user.PlayerSocket.Send(data);
                Thread.Sleep(200);
            }
        }

        /* Hàm xử lý việc sau khi đánh 1 lá bài và chuyển lượt */
        private static void HandleDanhBai(string[] Message, PLAYER User)
        {
            BOBAI.Current = Message[3]; //Cập nhật lá bài hiện tại
            PLAYERLIST[HienTai - 1].SoLuongBai = int.Parse(Message[2]); //Lấy số lượng bài còn lại của người chơi sau khi đánh đó
            if (PLAYERLIST[HienTai - 1].SoLuongBai == 0) //Kiểm tra nếu số lượng bài còn lại của người chơi sau khi đánh đó là 0
            {
                TrangThai = false;
                Console.WriteLine("Trạng thái game bây giờ " + TrangThai);
                WinnerName = PLAYERLIST[HienTai - 1].ID; //Lưu tên người thắng
                //Gửi thông điệp cho tất cả người chơi End: kết thúc game và bật màn hình kết quả thắng thua, người thắng (Message[1]) sẽ mở màn hình thắng, còn lại màn hình thua
                foreach (var user in PLAYERLIST)
                {
                    string SendData = "End;" + Message[1];
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                    user.PlayerSocket.Send(data);
                    Thread.Sleep(200);
                }
            }
            else
            {
                //Gửi thông điệp cho phép enable nút hô Uno bên client đó.
                //Sẽ theo cơ chế nếu người đó quên ấn nút hô Uno kể từ khi hết lượt của người đó còn 1 lá cho đến khi lại đến lượt của mình thì sẽ bị phạt rút thêm 2 lá (có thông báo bị phạt gì đó) và chuyển lượt.
                if (PLAYERLIST[HienTai - 1].SoLuongBai == 1)
                {
                    YELLUNOLIST.Add(Message[1]);
                    string SendData ="YellUNOEnable;" + Message[1];
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                    PLAYERLIST[HienTai - 1].PlayerSocket.Send(data);
                }
                //Gửi thông điệp Update: Cập nhật lá bài mới đánh ra và số lượng bài còn lại của người chơi đó cho toàn bộ người chơi 
                foreach (var user in PLAYERLIST)
                {
                    string SendData = "Update;" + Message[1] + ";" + Message[2] + ";" + Message[3];
                    if (Message[3].Contains("Wild_Draw") || Message[3].Contains("Wild")) //Đáng lý là chỉ cần contains Wild nhưng t trình bày hết cho dễ hiểu trường hợp này do chỉ có 2 lá đó là có màu được chọn đi kèm
                    {
                        SendData += ";" + Message[4];
                    }
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                    user.PlayerSocket.Send(data);
                    Thread.Sleep(200);
                }
                if (Message[3].Contains("Blue_Draw") || Message[3].Contains("Red_Draw") || Message[3].Contains("Green_Draw") || Message[3].Contains("Yellow_Draw")) //Nếu lá bài người chơi đánh là draw 2 (sử dụng Contains() xác nhận trong lá bài có phần cần tìm tương ứng)
                    RUT += 2;
                if (Message[3].Contains("Wild_Draw")) //Nếu lá bài người chơi đánh là draw 4 (sử dụng Contains() xác nhận trong lá bài có phần cần tìm tương ứng)               
                    RUT += 4;                
                if (Message[3].Contains("Reverse")) //Nếu lá bài người chơi đánh là reverse (sử dụng Contains() xác nhận trong lá bài có phần cần tìm tương ứng)
                {
                    if (ChieuDanh == true) //Đang thuận chiều thì ngược chiều và ngược lại
                        ChieuDanh = false;
                    else
                        ChieuDanh = true;
                }
                if (ChieuDanh == true) //Nếu thuận chiều 
                {
                    if (Message[3].Contains("Skip")) //Nếu lá bài người chơi đánh là skip
                    {
                        if (HienTai == PLAYERLIST.Count) //Nếu HienTai là người chơi có thứ tự lượt cuối cùng trong PLAYERLIST đã sắp xếp thứ tự theo lượt chơi
                        {
                            HienTai = 2; //HienTai sẽ là người chơi có thứ tự 2 trong PLAYERLIST
                        }
                        else
                        {
                            HienTai = HienTai + 2; //HienTai sẽ là người chơi kế người chơi ở lượt tiếp theo
                        }
                    }
                    else
                    {
                        HienTai++; //HienTai sẽ là người chơi ở lượt tiếp theo như bth
                    }
                }
                else //Nếu ngược chiều
                {
                    if (Message[3].Contains("Skip")) //Nếu lá bài người chơi đánh là skip
                    {
                        if (HienTai == 1) //Nếu HienTai là người chơi có thứ tự lượt đầu tiên trong PLAYERLIST đã sắp xếp thứ tự theo lượt chơi
                        {
                            HienTai = PLAYERLIST.Count - 1; //HienTai sẽ là người chơi có thứ tự cuối cùng trong PLAYERLIST 
                        }
                        else
                        {
                            HienTai = HienTai - 2; //HienTai sẽ là người chơi kế người chơi ở lượt tiếp theo
                        }
                    }
                    else
                    {
                        HienTai--; //HienTai sẽ là người chơi ở lượt tiếp theo như bth
                    }
                }
                if (HienTai > PLAYERLIST.Count) //Nếu HienTai sau khi tính toán qua các điều kiện trên vượt quá số người trong PLAYERLIST thì đến lượt người chơi đầu tiên trong PLAYERLIST
                    HienTai = 1;
                if (HienTai < 1) //Nếu HienTai sau khi tính toán qua các điều kiện trên nhỏ số người trong PLAYERLIST thì đến lượt người chơi đầu tiên trong PLAYERLIST
                    HienTai = PLAYERLIST.Count;
                //Nếu người chơi lượt kế tiếp là người còn 1 lá bài nhưng vẫn chưa hô Uno, gửi thông điệp xử phạt người chơi đó
                if (YELLUNOLIST.Contains(PLAYERLIST[HienTai - 1].ID))
                {
                    string SendData = "Penalty;" + PLAYERLIST[HienTai - 1].ID;
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                    PLAYERLIST[HienTai - 1].PlayerSocket.Send(data);
                    //Gửi thông điệp cho tất cả người chơi ngoại trừ người chơi bị phạt Turn: Gửi thông điệp về việc đến lượt của người chơi nào
                    foreach (var user in PLAYERLIST)
                    {
                        if (user.ID != PLAYERLIST[HienTai - 1].ID)
                        {
                            string SendData_ = "Turn;" + PLAYERLIST[HienTai - 1].ID;
                            byte[] data_ = Encoding.UTF8.GetBytes(Encrypt(SendData_));
                            user.PlayerSocket.Send(data_);
                            Thread.Sleep(200);
                        }                      
                    }
                }
                else
                {
                    //Gửi thông điệp cho tất cả người chơi Turn: Gửi thông điệp về việc đến lượt của người chơi nào
                    foreach (var user in PLAYERLIST)
                    {
                        string SendData = "Turn;" + PLAYERLIST[HienTai - 1].ID;
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        user.PlayerSocket.Send(data);
                        Thread.Sleep(200);
                    }
                }
            }
        }

        /* Hàm xử lý mỗi lần người chơi rút 1 bài và chuyển lượt */
        private static void HandleRutBai(string[] Message, PLAYER User)
        {
            PLAYERLIST[HienTai - 1].SoLuongBai = int.Parse(Message[2]); //Lấy thông tin về số bài còn lại của người chơi hiện tại
            if (BOBAI.CardName.Length == 0) // Nếu CardName đã rỗng
            {
                BOBAI.ResetCardName(); // Làm mới CardName
                ShuffleCards(); //Trộn lại bộ bài mới
            }
            string SendData = "CardDraw;" + Message[1] + ";" + BOBAI.CardName[0]; //Tạo chuỗi SendData chứa thông điệp CardDraw: bài mà người chơi rút được
            BOBAI.CardName = BOBAI.CardName.Where(val => val != BOBAI.CardName[0]).ToArray(); //Xóa lá bài đã rút ra khỏi mảng CardName
            byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData)); //Tạo mảng byte tên data chứa thông điệp CardDraw
            PLAYERLIST[HienTai - 1].PlayerSocket.Send(data); //Gửi thông điệp CardDraw đến người chơi rút bài
            Console.WriteLine("Note: " + SendData);
            //Gửi thông điệp Update: cập nhật số lượng bài mới của người chơi đó cho toàn bộ người chơi
            foreach (var user in PLAYERLIST)
            {
                string SendData_ = "Update;" + Message[1] + ";" + Message[2];
                byte[] data_ = Encoding.UTF8.GetBytes(Encrypt(SendData_));
                user.PlayerSocket.Send(data_);
                Thread.Sleep(200);
            }
            UpdateTurn();
        }

        /* Hàm xử lý việc bị rút nhiều lá bài do các lá bài đặc biệt hoặc bị phạt do không hô uno và chuyển lượt */
        private static void HandleSpecialDraw(string[] Message, PLAYER User)
        {
            PLAYERLIST[HienTai - 1].SoLuongBai = int.Parse(Message[2]); //Lấy thông tin về số bài còn lại của người chơi hiện tại
            string cardstack = "Specialdraws;" + PLAYERLIST[HienTai - 1].ID; //Tạo chuỗi cardstack chứa thông điệp Specialdraws: Các lá bài mà người chơi nhận được
            //Phạt người chơi không hô UNO rút thêm 2 lá
            if (YELLUNOLIST.Contains(PLAYERLIST[HienTai - 1].ID))
            {
                RUT += 2;
                YELLUNOLIST.Remove(PLAYERLIST[HienTai - 1].ID);
            }
            if (BOBAI.CardName.Length < RUT) //Nếu CardName còn số bài ít hơn số lượng rút
            {
                BOBAI.ResetCardName(); // Làm mới CardName
                ShuffleCards(); //Trộn lại bộ bài mới
            }    
            //Vòng lặp nối các lá bài vào cardstack để hoàn chỉnh SpecialDraw 
            for (int i = 0; i < RUT; i++)
            {
                cardstack += ";" + BOBAI.CardName[0];
                BOBAI.CardName = BOBAI.CardName.Where(val => val != BOBAI.CardName[0]).ToArray();
            }
            byte[] data = Encoding.UTF8.GetBytes(Encrypt(cardstack));
            PLAYERLIST[HienTai - 1].PlayerSocket.Send(data); //Gửi thông điệp SpecialDraw đến người chơi rút bài
            RUT = 0;
            Console.WriteLine("Note: " + cardstack);
            //Gửi thông điệp Update: cập nhật số lượng bài mới của người chơi đó cho toàn bộ người chơi
            foreach (var user in PLAYERLIST)
            {
                string SendData_ = "Update;" + Message[1] + ";" + Message[2];
                byte[] data_ = Encoding.UTF8.GetBytes(Encrypt(SendData_));
                user.PlayerSocket.Send(data_);
                Thread.Sleep(200);
            }
            UpdateTurn();
        }

        /* Hàm xử lý tin nhắn chat */
        private static void HandleChat(string[] Message, PLAYER User)
        {
            string sender = Message[1]; //Tạo chuỗi sender để lưu ID của người gửi tin chat 
            string ChatContent = Message[2]; //Tạo chuỗi MessContent để lưu nội dung tin chat trong mảng chuỗi Message
            //Gửi thông điệp ChatMessage: Tin chat của người chơi gửi chat đến tất cả người chơi còn lại
            foreach (var user in PLAYERLIST)
            {
                if (user.PlayerSocket != User.PlayerSocket)
                {
                    string SendData = $"ChatMessage;{sender};{ChatContent}";
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                    user.PlayerSocket.Send(data);
                }
            }
            Console.WriteLine($"{sender}: {ChatContent}");
        }

        /* Hàm cập nhật lượt kế tiếp */
        private static void UpdateTurn()
        {
            if (ChieuDanh == true) //Điều kiện để đổi chiều đánh
            {
                HienTai++;
            }
            else
            {
                HienTai--;
            }
            if (HienTai > PLAYERLIST.Count) // Nếu HienTai vượt quá số người chơi
                HienTai = 1; // Quay lại người chơi đầu tiên
            if (HienTai < 1) // Nếu HienTai giảm xuống dưới 1
                HienTai = PLAYERLIST.Count; // Quay về người chơi cuối cùng
            //Nếu người chơi lượt kế tiếp là người còn 1 lá bài nhưng vẫn chưa hô Uno, gửi thông điệp xử phạt người chơi đó
            if (YELLUNOLIST.Contains(PLAYERLIST[HienTai - 1].ID))
            {
                string SendData = "Penalty;" + PLAYERLIST[HienTai - 1].ID;
                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                PLAYERLIST[HienTai - 1].PlayerSocket.Send(data);
                //Gửi thông điệp cho tất cả người chơi ngoại trừ người chơi bị phạt Turn: Gửi thông điệp về việc đến lượt của người chơi nào
                foreach (var user in PLAYERLIST)
                {
                    if(user.ID != PLAYERLIST[HienTai - 1].ID)
                    {
                        string SendData_ = "Turn;" + PLAYERLIST[HienTai - 1].ID;
                        byte[] data_ = Encoding.UTF8.GetBytes(Encrypt(SendData_));
                        user.PlayerSocket.Send(data_);
                        Thread.Sleep(200);
                    }                      
                }
            }
            else
            {
                //Gửi thông điệp cho tất cả người chơi Turn: Gửi thông điệp về việc đến lượt của người chơi nào
                foreach (var user in PLAYERLIST)
                {
                    string SendData = "Turn;" + PLAYERLIST[HienTai - 1].ID;
                    byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                    user.PlayerSocket.Send(data);
                    Thread.Sleep(200);
                }
            }
        }

        /* Hàm cập nhật điểm của người thắng sau khi hết ván */
        private static void HandleDiem(string[] Message, PLAYER User)
        {
            for (int i = 0; i < PLAYERLIST.Count; i++)
            {
                if (PLAYERLIST[i].ID == WinnerName)
                {
                    PLAYERLIST[i].Diem += int.Parse(Message[2]);
                    Console.WriteLine("Điểm của người thắng " + WinnerName + ": " + PLAYERLIST[i].Diem);
                    break;
                }
            }
        }

        /* Hàm xử lý restart hoặc finish game */
        private static void HandleAfterMatch(string[] Message, PLAYER User)
        {
            //Đếm Restart hoặc Finish 
            for (int i = 0; i < PLAYERLIST.Count; i++)
            {
                if (PLAYERLIST[i].ID == Message[1])
                {
                    if (Message[0] == "Restart")
                    {
                        DemRestart += 1;
                        Console.WriteLine(Message[1] + " đã chọn Restart");
                        break;
                    }                       
                    if (Message[0] == "Finish")
                    {
                        DemFinish += 1;
                        Console.WriteLine(Message[1] + " đã chọn Finish");
                        break;
                    }                          
                }
            }
            //Nếu đủ số người đã chọn thì quyết định restart hay hiện màn hình xếp hạng kết thúc.
            if ((DemRestart + DemFinish) == PLAYERLIST.Count)
            {
                if (DemFinish > 0) //Có người chọn Finish thì gưi thông điệp Result: hiện màn hình xếp hạng kết thúc và điểm số.
                {
                    PLAYERLIST.Sort((x, y) => x.Diem.CompareTo(y.Diem)); //Sắp xếp lại PLAYERLIST theo điểm tăng dần
                    for (int i = 0; i < PLAYERLIST.Count; i++)
                        PLAYERLIST[i].Rank = PLAYERLIST.Count - i; //Gán hạng của người chơi (4-1)
                    //Gửi thông điệp result chứa thông tin từng người chơi cho từng người chơi tương ứng
                    foreach (var user in PLAYERLIST)
                    {
                        string SendData = "Result;" + user.ID + ";" + user.Diem + ";" + user.Rank;
                        byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                        user.PlayerSocket.Send(data);
                        Thread.Sleep(200);
                    }
                    //Gửi thông điệp result chứa thông tin những người chơi khác cho mỗi người chơi (để thuận lợi tạo bảng xếp hạng bên client)
                    //Ví dụ t là người chơi thì sẽ gửi thông tin result của những người còn lại cho t để bên client của t cập nhập bảng xếp hạng.
                    foreach (var user in PLAYERLIST)
                    {
                        foreach (var player_ in PLAYERLIST)
                        {
                            if (user.ID != player_.ID)
                            {
                                string SendData = "Result;" + player_.ID + ";" + player_.Diem + ";" + player_.Rank;
                                byte[] data = Encoding.UTF8.GetBytes(Encrypt(SendData));
                                user.PlayerSocket.Send(data);
                                Thread.Sleep(200);
                            }
                        }
                    }
                    Console.WriteLine("Game đã kết thúc hoàn toàn! Bye bye.");
                }
                else
                {
                    HienTai = 1;
                    ChieuDanh = true;
                    RUT = 0;
                    YELLUNOLIST.Clear();
                    DemFinish = 0;
                    DemRestart = 0;
                    WinnerName = "";
                    SetupGame(Message, User);  //Đủ người thì lại thiết lập bắt đầu trò chơi
                    Console.WriteLine("Đủ người chơi muốn restart, bắt đầu lại...");
                }
            }
        }

        /* Hàm xử lý disconnect */
        private static void HandleDisconnect(string[] Message, PLAYER User)
        {         
            User.PlayerSocket.Shutdown(SocketShutdown.Both);
            User.PlayerSocket.Close();
            Console.WriteLine(Message[1] + " đã thoát.");
            PLAYERLIST0.Remove(User);
        }
    }
}
