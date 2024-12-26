using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOServer
{
    /* Class chứa các lá bài của bộ bài */
    internal class BOBAI
    {
        public static string Current = "";
        public static string[] DefaultCardName =
        {
                "Red_0", "Red_1", "Red_2", "Red_3", "Red_4", "Red_5", "Red_6", "Red_7", "Red_8", "Red_9", "Red_1_", "Red_2_", "Red_3_", "Red_4_", "Red_5_", "Red_6_", "Red_7_", "Red_8_", "Red_9_",
                "Blue_0", "Blue_1", "Blue_2", "Blue_3", "Blue_4", "Blue_5", "Blue_6", "Blue_7", "Blue_8", "Blue_9", "Blue_1_", "Blue_2_", "Blue_3_", "Blue_4_", "Blue_5_", "Blue_6_", "Blue_7_", "Blue_8_", "Blue_9_",
                "Yellow_0", "Yellow_1", "Yellow_2", "Yellow_3", "Yellow_4", "Yellow_5", "Yellow_6", "Yellow_7", "Yellow_8", "Yellow_9", "Yellow_1_", "Yellow_2_", "Yellow_3_", "Yellow_4_", "Yellow_5_", "Yellow_6_", "Yellow_7_", "Yellow_8_", "Yellow_9_",
                "Green_0", "Green_1", "Green_2", "Green_3", "Green_4", "Green_5", "Green_6", "Green_7", "Green_8", "Green_9", "Green_1_", "Green_2_", "Green_3_", "Green_4_", "Green_5_", "Green_6_", "Green_7_", "Green_8_", "Green_9_",
                "Blue_Reverse", "Blue_Reverse_", "Red_Reverse", "Red_Reverse_", "Yellow_Reverse", "Yellow_Reverse_", "Green_Reverse", "Green_Reverse_",
                "Blue_Skip", "Blue_Skip_", "Red_Skip", "Red_Skip_", "Yellow_Skip", "Yellow_Skip_", "Green_Skip", "Green_Skip_",
                "Wild_1", "Wild_2", "Wild_3", "Wild_4",
                "Wild_Draw_1", "Wild_Draw_2", "Wild_Draw_3", "Wild_Draw_4",
                "Blue_Draw", "Blue_Draw_", "Red_Draw", "Red_Draw_", "Yellow_Draw", "Yellow_Draw_", "Green_Draw", "Green_Draw_"
        }; //Mảng string gốc chứa tên các lá bài trong bộ bài
        public static string[] CardName;
        // Hàm khởi tạo lại CardName
        public static void ResetCardName()
        {
            CardName = (string[])DefaultCardName.Clone();
        }
    }
}
