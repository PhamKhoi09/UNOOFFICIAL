using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using UnoOnline;

public class CustomCardPanel : Panel
{
    private GameManager gameManager;
    private Player currentPlayer; // Declare currentPlayer

    private List<Card> cards;
    private int hoveredCardIndex = -1;


    private const int CARD_WIDTH = 71;
    private const int CARD_HEIGHT = 96;
    private const int CARD_SPACING = 10;
    private const int HOVER_LIFT = 20;


    public CustomCardPanel()
    {
        this.DoubleBuffered = true;
        cards = new List<Card>();
        this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        this.BackColor = Color.FromArgb(100, 255, 255, 255); // Alpha = 100 cho độ trong suốt
    }
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x20;  // WS_EX_TRANSPARENT
            return cp;
        }
    }

    public CustomCardPanel(GameManager manager)
    {
        this.DoubleBuffered = true;
        cards = new List<Card>();
        gameManager = manager; // Assign GameManager
    }

    public CustomCardPanel(GameManager manager, Player player)
    {
        this.DoubleBuffered = true;
        cards = new List<Card>();
        gameManager = manager; // Assign GameManager
        currentPlayer = player; // Assign Player
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        for (int i = 0; i < cards.Count; i++)
        {
            int x = i * CARD_SPACING;
            int y = this.Height - 150; // Base position

            // Lift card if hovered
            if (i == hoveredCardIndex)
                y -= HOVER_LIFT;

            DrawCard(g, cards[i], x, y);
        }
    }

    private void DrawCard(Graphics g, Card card, int x, int y)
    {
        // Load card image
        Image cardImage = GetCardImage(card);

        // Add shadow effect
        using (var shadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
        {
            g.FillRectangle(shadow, x + 5, y + 5, cardImage.Width, cardImage.Height);
        }

        // Draw card with smooth edges
        g.DrawImage(cardImage, x, y);

        // Check if the card can be played
        if (gameManager.IsValidMove(card))
        {
            using (var glow = new Pen(Color.Yellow, 2))
            {
                g.DrawRectangle(glow, x, y, cardImage.Width, cardImage.Height);
            }
        }
    }


    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        // Calculate hovered card
        int newHoverIndex = (e.X / CARD_SPACING);
        if (newHoverIndex != hoveredCardIndex && newHoverIndex < cards.Count)
        {
            hoveredCardIndex = newHoverIndex;
            this.Invalidate();
        }
    }

    public void SetCards(List<Card> newCards)
    {
        cards = newCards;
        this.Invalidate();
    }

    private Image GetCardImage(Card card)
    {
        // Implement logic to load card image based on card properties
        return null; // Placeholder
    }

    private Card _card;
    private bool _isPlayable;
    private bool _isHovered;
    private const int HOVER_OFFSET = 20;

    public CustomCardPanel(Card card) : base()
    {
        _card = card;
        this.Size = new Size(CARD_WIDTH, CARD_HEIGHT);
        this.Margin = new Padding(CARD_SPACING, 0, 0, 0);
        this.BackgroundImageLayout = ImageLayout.Stretch;
        this.BackgroundImage = GetCardImage(card);
        InitializePanel();
        SetupEvents();
    }


    private void InitializePanel()
    {
        this.Size = new Size(71, 96);  // Giữ nguyên kích thước như cũ
        this.BackgroundImageLayout = ImageLayout.Stretch;
        this.BackgroundImage = GetCardImage(_card);
        this.Margin = new Padding(5, 5, 5, 5);
    }

    private void SetupEvents()
    {
        this.MouseEnter += (s, e) => {
            if (_isPlayable)
            {
                _isHovered = true;
                this.Top -= HOVER_OFFSET;
                this.Cursor = Cursors.Hand;
                this.Invalidate(); // Trigger paint để vẽ highlight effect
            }
        };

        this.MouseLeave += (s, e) => {
            if (_isHovered)
            {
                _isHovered = false;
                this.Top += HOVER_OFFSET;
                this.Cursor = Cursors.Default;
                this.Invalidate();
            }
        };
    }

    public void SetPlayable(bool playable)
    {
        _isPlayable = playable;
        this.Invalidate();
    }
}
