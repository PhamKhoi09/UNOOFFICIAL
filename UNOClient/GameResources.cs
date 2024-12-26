using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public static class GameResources
{
    // Dictionary lưu cache hình ảnh lá bài
    private static Dictionary<string, Image> cardImages = new Dictionary<string, Image>();

    public static Image GetCardImage(Card card)
    {
        string key;
        switch (card.CardType.ToLower())
        {
            case "normal":
                key = $"{card.Color}_{card.Value}";
                break;
            case "action":
                key = $"{card.Color}_{card.Value}";
                break;
            case "wild":
                key = card.Value.ToLower() == "draw four" ? "WildDrawFour" : "Wild";
                break;
            default:
                key = "Default";
                break;
        }



        if (!cardImages.ContainsKey(key))
        {
            string imagePath = $@"Resources\Cards\{key}.png";
            if (File.Exists(imagePath))
            {
                cardImages[key] = Image.FromFile(imagePath);
            }
            else
            {
                // Nếu không tìm thấy file, sử dụng một hình ảnh mặc định
                cardImages[key] = Image.FromFile(@"Resources\CardImages\Deck.png");
            }
        }
        return cardImages[key];
    }


    // Load các icons
    public static Image DrawCardIcon => Image.FromFile(@"Resources\Icons\draw.png");
    public static Image SkipIcon => Image.FromFile(@"Resources\Icons\skip.png");

    // Các màu sắc chủ đạo
    public static Color PrimaryColor => Color.FromArgb(52, 152, 219);
    public static Color SecondaryColor => Color.FromArgb(41, 128, 185);
}

