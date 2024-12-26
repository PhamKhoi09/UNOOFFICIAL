using System.Drawing;
using System.Windows.Forms;
using System;

public class Card
{
    public string Color { get; set; }
    public string Value { get; set; }
    public string CardType { get; set; }  // "Number", "Wild", "Action"
    public string CardName { get; set; }

    // Constructor mặc định
    public Card()
    {
        DetermineCardType(); // Tự động xác định loại thẻ khi tạo mới
    }

    // Constructor với đầy đủ tham số
    public Card(string cardName, string color, string value)
    {
        CardName = cardName;
        Color = color;
        Value = value;
        DetermineCardType();
    }

    // Phương thức tự động xác định loại thẻ
    public void DetermineCardType()
    {
        if (Color == "Wild")
        {
            CardType = "Wild";
        }
        else if (Value == "Skip" || Value == "Reverse" || Value == "Draw" || Value == "Draw Two")
        {
            CardType = "Action";
        }
        else
        {
            CardType = "Number";
        }
    }

    public bool IsDrawCard => Value == "Draw" || Value == "Draw Two";
    public bool IsWildCard => Color == "Wild" && Value == "Wild";
    public bool IsWildDrawFour => Color == "Wild" && Value == "Draw Four";

    public override string ToString()
    {
        return $"{Color} {Value}";
    }

    public class CardPictureBox : PictureBox
    {
        private bool isHovered = false;

        public CardPictureBox()
        {
            this.MouseEnter += Card_MouseEnter;
            this.MouseLeave += Card_MouseLeave;
        }

        private void Card_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;
            // Di chuyển thẻ lên một chút khi hover
            this.Top -= 10;
            this.Refresh();
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false;
            // Đưa thẻ về vị trí cũ
            this.Top += 10;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (isHovered)
            {
                // Thêm viền sáng khi hover
                using (Pen pen = new Pen(System.Drawing.Color.Yellow, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            }
        }
    }
    

}
