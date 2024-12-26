using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UNOServer
{
    /* Class để lưu trữ thông tin từng người chơi */
    internal class PLAYER
    {
        public string ID { get; set; } //Danh tính (số ID) của người chơi
        public int SoLuongBai { get; set; } //Số lượng bài của người chơi
        public int Luot { get; set; } //Thứ tự (lượt) của người chơi 
        public int Diem { get; set; } //Điểm người chơi
        public int Rank { get; set; } //Hạng người chơi
        public Socket PlayerSocket { get; set; } //Socket người chơi 
    }
}
