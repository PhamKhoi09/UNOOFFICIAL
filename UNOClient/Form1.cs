    using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UnoOnline
{
    public partial class Form1 : Form
    {
        //private Label currentPlayerLabel;
        //private ProgressBar turnTimer;
        //private Label currentCardLabel;
        //private Panel PlayerHandPanel;
        //private Button skipTurnButton;
        //private Button drawCardButton;
        private Panel chatPanel;
        private TextBox chatInput;
        private RichTextBox chatHistory;
        private Timer timer;
        private Button yellUNOButton;
        private TableLayoutPanel mainLayout;
        private Panel gameStatusPanel;
        private FlowLayoutPanel playerCardsPanel;
        private Panel actionPanel;
        Card currentCard = GameManager.Instance.CurrentCard;

        private Button redButton;
        private Button greenButton;
        private Button yellowButton;
        private Button blueButton;

        // Update the image displayed in the PictureBox
        public void UpdateCurrentCardDisplay(Card currentCard)
        {
            if (currentCard == null)
            {
                MessageBox.Show("CurrentCard is null in UpdateCurrentCardDisplay.");
                return;
            }
            string cardImagePath = "";
            if (currentCard.CardName.Contains("Wild"))
            {
                if (currentCard.Value == "Draw")
                    cardImagePath = Path.Combine("Resources", "CardImages", "Wild_Draw.png");
                else
                    cardImagePath = Path.Combine("Resources", "CardImages", "Wild.png");
            }
            else
            {
                // Construct the file path for the card image
                cardImagePath = Path.Combine("Resources", "CardImages", $"{currentCard.Color}_{currentCard.Value}.png");
            }

            if (File.Exists(cardImagePath))
            {
                // Load the image into the PictureBox
                currentCardPictureBox.Image = Image.FromFile(cardImagePath);
            }
        }
        private void InitializeAdditionalComponents()
        {
            // Panel chat (bên phải)
            chatPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 250
            };

            chatHistory = new RichTextBox
            {
                Dock = DockStyle.Top,
                Height = ClientSize.Height - 50,
                ReadOnly = true,
                BackColor = Color.White
            };

            chatInput = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 30
            };
            // Thêm controls chat vào panel
            chatPanel.Controls.AddRange(new Control[] { chatHistory, chatInput });

            // Thêm các controls mới vào form
            Controls.AddRange(new Control[]
            {
            chatPanel
            });

            // Đăng ký events
            //chatInput.KeyPress += ChatInput_KeyPress;
        }

        // Thêm xử lý chat
        //private void ChatInput_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == (char)Keys.Enter)
        //    {
        //        e.Handled = true;
        //        string message = chatInput.Text.Trim();
        //        if (!string.IsNullOrEmpty(message))
        //        {
        //            ClientSocket.SendMessage($"CHAT|{message}");
        //            chatInput.Clear();
        //        }
        //    }
        //}


        // Thêm method hiển thị chat
        public void AddChatMessage(string sender, string message)
        {
            if (chatHistory.InvokeRequired)
            {
                BeginInvoke(new Action(() => AddChatMessage(sender, message)));
                return;
            }
            string formattedMessage = $"[{sender}]: {message}\n";
            chatHistory.AppendText(formattedMessage);
            chatHistory.ScrollToCaret();
        }

        private bool GetAutoScroll()
        {
            return AutoScroll;
        }

        //    private void InitializeGameLayout(bool autoScroll)
        //    {
        //        this.ClientSize = new Size(1280, 720);
        //        this.StartPosition = FormStartPosition.CenterScreen;
        //        this.TransparencyKey = Color.FromArgb(0, 0, 0); // Màu trong suốt

        //        // Panel chứa bài của người chơi (có tính năng cuộn)
        //        Panel playerHandPanel = new Panel
        //        {
        //            Location = new Point(180, this.ClientSize.Height - 150),
        //            Size = new Size(920, 130), // Kích thước panel
        //            AutoScroll = true, // Bật tính năng cuộn
        //            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
        //            BackColor = Color.Transparent // Màu nền

        //        };



        //        // Thêm panel vào form
        //        this.Controls.Add(playerHandPanel);

        //        // Tạo và thêm thẻ vào playerHandPanel
        //        int cardWidth = 70; // Kích thước thẻ
        //        int cardHeight = 100; // Kích thước thẻ
        //        int spacing = 10; // Khoảng cách giữa các thẻ
        //        int numberOfCards = 15; // Số thẻ bạn muốn hiển thị

        //        for (int i = 0; i < numberOfCards; i++)
        //        {
        //            CustomCard card = new CustomCard
        //            {
        //                Size = new Size(cardWidth, cardHeight),
        //                Location = new Point(i * (cardWidth + spacing), 0) // Sắp xếp theo hàng ngang
        //            };
        //            playerHandPanel.Controls.Add(card);
        //        }

        //        // Panel hiển thị bài trên bàn (ở giữa)
        //        CustomCardPanel tableDeckPanel = new CustomCardPanel
        //        {
        //            Location = new Point(540, 260),
        //            Size = new Size(200, 200),
        //            Anchor = AnchorStyles.None,
        //            BackColor = Color.FromArgb(100, 0, 0, 0) // Màu nền với độ trong suốt


        //        };

        //        // Panel hiển thị người chơi đối thủ (phía trên)
        //        CustomCardPanel opponentPanel = new CustomCardPanel
        //        {
        //            Location = new Point(180, 20),
        //            Size = new Size(920, 130),
        //            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
        //            BackColor = Color.FromArgb(100, 0, 0, 0) // Màu nền với độ trong suốt

        //        };


        //        // Panel thông tin game bên trái
        //        Panel gameInfoPanel = new Panel
        //        {
        //            Location = new Point(20, 160),
        //            Size = new Size(150, 400),
        //            Anchor = AnchorStyles.Left,
        //            BackColor = Color.FromArgb(100, 0, 0, 0),

        //        };



        //        // Thêm các controls thông tin vào gameInfoPanel
        //        Label currentPlayerLabel = new Label
        //        {
        //            Location = new Point(10, 10),
        //            Size = new Size(130, 25),
        //            Text = "Current Player:",
        //            ForeColor = Color.White,
        //            Font = new Font("Segoe UI", 12F)
        //        };

        //        Label scoreLabel = new Label
        //        {
        //            Location = new Point(10, 45),
        //            Size = new Size(130, 25),
        //            Text = "Score: 0",
        //            ForeColor = Color.White,
        //            Font = new Font("Segoe UI", 12F)
        //        };

        //        // Panel chứa các nút điều khiển (bên phải)
        //        Panel controlPanel = new Panel
        //        {
        //            Location = new Point(this.ClientSize.Width - 170, 160),
        //            Size = new Size(150, 400),
        //            Anchor = AnchorStyles.Right,
        //            BackColor = Color.FromArgb(100, 0, 0, 0)
        //        };

        //        // Thêm các nút vào controlPanel
        //        Button drawCardButton = new Button
        //        {
        //            Location = new Point(10, 10),
        //            Size = new Size(130, 40),
        //            Text = "Draw Card",
        //            FlatStyle = FlatStyle.Flat,
        //            BackColor = Color.FromArgb(100, 0, 122, 204),
        //            ForeColor = Color.White,
        //            Font = new Font("Segoe UI", 12F)
        //        };

        //        Button unoButton = new Button
        //        {
        //            Location = new Point(10, 60),
        //            Size = new Size(130, 40),
        //            Text = "UNO!",
        //            FlatStyle = FlatStyle.Flat,
        //            BackColor = Color.FromArgb(100, 204, 0, 0),
        //            ForeColor = Color.White,
        //            Font = new Font("Segoe UI", 12F)
        //        };

        //        // Thêm chat box và input (phía dưới bên phải)
        //        RichTextBox chatBox = new RichTextBox
        //        {
        //            Location = new Point(this.ClientSize.Width - 300, this.ClientSize.Height - 200),
        //            Size = new Size(280, 150),
        //            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
        //            BackColor = Color.FromArgb(150, 255, 255, 255),
        //            ReadOnly = true
        //        };

        //        TextBox chatInput = new TextBox
        //        {
        //            Location = new Point(this.ClientSize.Width - 300, this.ClientSize.Height - 40),
        //            Size = new Size(280, 30),
        //            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
        //            BackColor = Color.FromArgb(150, 255, 255, 255)
        //        };

        //        // Thêm các controls vào form
        //        this.Controls.AddRange(new Control[] {
        //            playerHandPanel,
        //            tableDeckPanel,
        //            opponentPanel,
        //            gameInfoPanel,
        //            controlPanel,
        //            chatBox,
        //            chatInput
        //         });

        //        // Thêm controls vào các panel
        //        gameInfoPanel.Controls.AddRange(new Control[] {
        //    currentPlayerLabel,
        //    scoreLabel
        //});
        //        controlPanel.Controls.AddRange(new Control[] {
        //    drawCardButton,
        //    unoButton
        //});

        //        drawCardButton.Click += DrawCardButton_Click;

        //        // Main layout chia làm 3 phần: status, game area, player cards
        //        mainLayout = new TableLayoutPanel
        //        {
        //            Dock = DockStyle.Fill,
        //            RowCount = 3,
        //            ColumnCount = 1,
        //            BackColor = Color.FromArgb(41, 128, 185) // Màu xanh dương đậm
        //        };

        //        // Game status panel
        //        gameStatusPanel = new Panel
        //        {
        //            Height = 60,
        //            Dock = DockStyle.Top,
        //            BackColor = Color.FromArgb(52, 73, 94)
        //        };

        //        // Khu vực chơi bài chính
        //        Panel gameArea = new Panel
        //        {
        //            Dock = DockStyle.Fill,
        //            Padding = new Padding(20)
        //        };

        //        // Panel chứa bài của người chơi
        //        playerCardsPanel = new FlowLayoutPanel
        //        {
        //            Dock = DockStyle.Bottom,
        //            Height = 150,
        //            FlowDirection = FlowDirection.LeftToRight,
        //            WrapContents = false,
        //            AutoScroll = true,
        //            Location = new Point(0, 750), // Vị trí nằm phía dưới cửa sổ (1600x900)
        //            Size = new Size(1600, 150), // Kích thước Panel
        //            BorderStyle = BorderStyle.FixedSingle, // Đặt viền cho Panel để dễ quan sát
        //            BackColor = Color.LightGray // Màu nền tùy chọn
        //        };

        //        // Panel chứa các nút action
        //        actionPanel = new Panel
        //        {
        //            Dock = DockStyle.Right,
        //            Width = 150,
        //            BackColor = Color.FromArgb(44, 62, 80)
        //        };

        //        // Setup các controls
        //        SetupGameStatusPanel();
        //        SetupActionPanel();

        //        // Thêm vào form
        //        mainLayout.Controls.Add(gameStatusPanel, 0, 0);
        //        mainLayout.Controls.Add(gameArea, 0, 1);
        //        mainLayout.Controls.Add(playerCardsPanel, 0, 2);

        //        this.Controls.Add(mainLayout);
        //    }

        private Panel CreateControlPanel()
        {
            Panel controlPanel = new Panel
            {
                Location = new Point(this.ClientSize.Width - 170, 160),
                Size = new Size(150, 400),
                Anchor = AnchorStyles.Right,
                BackColor = Color.FromArgb(100, 0, 0, 0) // Màu nền với độ trong suốt
            };

            Button drawCardButton = new Button
            {
                Location = new Point(10, 10),
                Size = new Size(130, 40),
                Text = "Draw Card",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F)
            };

            Button unoButton = new Button
            {
                Location = new Point(10, 60),
                Size = new Size(130, 40),
                Text = "UNO!",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 204, 0, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F)
            };

            controlPanel.Controls.AddRange(new Control[] { drawCardButton, unoButton });
            drawCardButton.Click += DrawCardButton_Click; // Gán sự kiện click
            return controlPanel;
        }

        public class CustomCard : UserControl
        {
            public CustomCard()
            {
                this.BackColor = Color.White; // Màu nền của thẻ
                this.BorderStyle = BorderStyle.FixedSingle; // Đường viền cho thẻ
            }
        }

        private void SetupGameStatusPanel()
        {
            Label turnLabel = new Label
            {
                Text = "Lượt của người chơi",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            Label currentCardLabel = new Label
            {
                Text = "Lá bài hiện tại:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 4),
                AutoSize = true,
                Location = new Point(10, 35)
            };

            gameStatusPanel.Controls.AddRange(new Control[] { turnLabel, currentCardLabel });
        }

        private void SetupActionPanel()
        {
            // Tạo các button với style thống nhất
            Button drawButton = CreateStyledButton("Rút bài", 0);
            Button skipButton = CreateStyledButton("Bỏ qua", 1);
            Button unoButton = CreateStyledButton("UNO!", 2);

            actionPanel.Controls.AddRange(new Control[] { drawButton, skipButton, unoButton });
        }

        private Button CreateStyledButton(string text, int index)
        {
            return new Button
            {
                Text = text,
                Width = 130,
                Height = 40,
                Location = new Point(10, 10 + (index * 50)),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }


        private void ApplyCustomTheme()
        {
            // Set màu nền gradient
            this.Paint += (sender, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(41, 128, 185), // Màu xanh đậm
                    Color.FromArgb(44, 62, 80),   // Màu xám đen
                    90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Style cho buttons
            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    // Skip color buttons
                    if (btn.BackColor == Color.Red || btn.BackColor == Color.Green || btn.BackColor == Color.Yellow || btn.BackColor == Color.Blue)
                        continue;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;
                    btn.BackColor = Color.FromArgb(52, 152, 219);

                    // Hover effect
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
                    btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 152, 219);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeGameBoard();
            InitializeTimer();
            ApplyCustomTheme();
            InitializeCustomComponents();
            this.ClientSize = new System.Drawing.Size(945, 540); // Kích thước nội dung (không bao gồm thanh tiêu đề và viền)
            this.StartPosition = FormStartPosition.CenterScreen; // Hiển thị Form ở giữa màn hình

            this.BackgroundImageLayout = ImageLayout.None; // Đảm bảo kích thước chính xác của hình ảnh được giữ nguyên
            if (this.BackgroundImage != null)
            {
                this.ClientSize = this.BackgroundImage.Size; // Đặt kích thước Form khớp với kích thước hình ảnh
            }
            // Tùy chọn: Ngăn thay đổi kích thước Form
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen; // Hiển thị Form ở giữa màn hình
        }

        public static void UpdateCurrentPlayerLabel(string playerName)
        {
            Form1 form = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form != null)
            {
                form.Invoke(new Action(() => form.currentPlayerLabel.Text = $"Lượt của: {playerName}"));
            }
        }

        private void InitializeGameBoard()
        {


            clientInfoLabel = new Label
            {
                Size = new Size(200, 30),
                Text = $"Tên: {Program.player.Name}",
                Font = new Font("Arial", 14),
                BackColor = Color.Transparent,
                Location = new Point(10, 10) // Góc trên bên trái
            };
            Controls.Add(clientInfoLabel);

            currentColor = new Label
            {
                Size = new Size(200, 30),
                Text = $"Màu hiện tại: {GameManager.Instance.CurrentCard.Color}",
                Font = new Font("Arial", 14),
                BackColor = Color.Transparent,
                Location = new Point(10, 40)
            };
            Controls.Add(currentColor);

            // PictureBox for current card
            currentCardPictureBox = new PictureBox
            {
                Size = new Size(this.ClientSize.Width / 6, this.ClientSize.Height / 3),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location = new Point((this.ClientSize.Width - this.ClientSize.Width / 6) / 2, (this.ClientSize.Height - this.ClientSize.Height / 3) / 2 - 35),
                BorderStyle = BorderStyle.FixedSingle,
            };
            Controls.Add(currentCardPictureBox);

            // Label for current player
            currentPlayerLabel = new Label
            {
                Size = new Size(200, 30),
                Text = $"Lượt của: {GameManager.Instance.Players[0].Name}",
                Font = new Font("Arial", 14),
                BackColor = Color.Transparent,
                Location = new Point(currentCardPictureBox.Left + (currentCardPictureBox.Width - 200) / 2, currentCardPictureBox.Top - 40)
            };
            Controls.Add(currentPlayerLabel);

            // Panel for player hand
            PlayerHandPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
            };
            Controls.Add(PlayerHandPanel);

            // Draw card button
            drawCardButton = new Button
            {
                Size = new Size(100, 40),
                Text = "Rút bài",
                Location = new Point(this.ClientSize.Width - 120, (this.ClientSize.Height - 200) / 2 + 50),
                BackColor = Color.Empty
            };
            drawCardButton.Click += DrawCardButton_Click;
            Controls.Add(drawCardButton);

            yellUNOButton = new Button
            {
                Size = new Size(100, 40),
                Text = "UNO!",
                Location = new Point(this.ClientSize.Width - 120, (this.ClientSize.Height - 200) / 2 + 100)
            };
            yellUNOButton.Click += yellUNOButton_Click;
            Controls.Add(yellUNOButton);

            // Initialize color buttons
            redButton = new Button
            {
                Size = new Size(50, 50),
                BackColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(currentCardPictureBox.Right + 10, currentCardPictureBox.Bottom - 110)
            };
            Controls.Add(redButton);
            redButton.Click += RedButton_Click;

            greenButton = new Button
            {
                Size = new Size(50, 50),
                BackColor = Color.Green,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(currentCardPictureBox.Right + 70, currentCardPictureBox.Bottom - 110)
            };
            Controls.Add(greenButton);
            greenButton.Click += GreenButton_Click;

            yellowButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(50, 50),
                BackColor = Color.Yellow,
                Location = new Point(currentCardPictureBox.Right + 10, currentCardPictureBox.Bottom - 50)
            };
            Controls.Add(yellowButton);
            yellowButton.Click += YellowButton_Click;

            blueButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(50, 50),
                BackColor = Color.Blue,
                Location = new Point(currentCardPictureBox.Right + 70, currentCardPictureBox.Bottom - 50)
            };
            Controls.Add(blueButton);
            blueButton.Click += BlueButton_Click;
            // Initialize deck images
            InitializeDeckImages();

            // Initialize chat panel
            InitializeChatPanel();
        }

        private void yellUNOButton_Click(object sender, EventArgs e)
        {
            Message yellUNOMessage = new Message(MessageType.YellUNO, new List<string> { Program.player.Name });
            ClientSocket.SendData(yellUNOMessage);
            // Disable uno button
            yellUNOButton.Enabled = false;
        }


        private void InitializeTimer()
        {
            // Khởi tạo Timer
            timer = new Timer();
            timer.Interval = 1000;  // Đặt thời gian đếm ngược mỗi giây
            timer.Tick += Timer_Tick;  // Gắn sự kiện Tick để giảm thời gian mỗi giây
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (turnTimer.Value > 0)
            {
                turnTimer.Value -= 10;  // Giảm giá trị ProgressBar mỗi giây
            }
            else
            {
                // Nếu hết thời gian, chuyển sang lượt tiếp theo
                //Phải thông  báo cho server biết nữa => gửi tin RutBai
            }
        }

        public void DisplayPlayerHand(List<Card> playerHand)
        {
            if (playerHand == null)
            {
                System.Diagnostics.Debug.WriteLine("Player hand cannot be null.");
                throw new ArgumentNullException(nameof(playerHand), "Player hand cannot be null.");
            }
            PlayerHandPanel.Controls.Clear(); // Clear existing controls
            int xOffset = 10;
            int yOffset = 10;
            int cardWidth = 100; // Increased card width
            int cardHeight = 150; // Increased card height

            foreach (var card in playerHand)
            {
                if (card == null)
                {
                    System.Diagnostics.Debug.WriteLine("Found a null card in player hand.");
                    throw new ArgumentException("Card cannot be null", nameof(card));
                }

                System.Diagnostics.Debug.WriteLine("Displaying card: " + card.CardName);

                Button cardButton = new Button
                {
                    Size = new Size(cardWidth, cardHeight),
                    Location = new Point(xOffset, yOffset),
                    BackgroundImage = GetCardImage(card),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    FlatStyle = FlatStyle.Flat,
                    Tag = card,
                    BackColor = Color.White,
                    FlatAppearance = { BorderSize = 1, BorderColor = Color.Black }
                };

                cardButton.FlatAppearance.MouseOverBackColor = Color.LightGray;
                cardButton.FlatAppearance.BorderSize = 1;

                cardButton.Click += CardButton_Click;

                PlayerHandPanel.Controls.Add(cardButton);

                xOffset += cardWidth + 5; // Space between cards
            }
        }
        private Image GetCardImage(Card card)
        {
            if (card == null)
            {
                MessageBox.Show("Card is null in GetCardImage.");
                return null;
            }

            string cardImagePath = "";

            // Xử lý các thẻ đặc biệt như "Wild"
            if (card.Color == "Wild")
            {
                if (card.Value == "Draw")
                    cardImagePath = Path.Combine("Resources", "CardImages", "Wild_Draw.png");
                else
                    cardImagePath = Path.Combine("Resources", "CardImages", "Wild.png");
            }
            else
            {
                // Đối với các lá bài màu
                cardImagePath = Path.Combine("Resources", "CardImages", $"{card.Color}_{card.Value}.png");
            }

            if (File.Exists(cardImagePath))
            {
                return Image.FromFile(cardImagePath);
            }
            else
            {
                MessageBox.Show($"Card image not found: {cardImagePath}");
                return null;
            }
        }
        private void EnableColorButtons()
        {
            redButton.Enabled = true;
            greenButton.Enabled = true;
            yellowButton.Enabled = true;
            blueButton.Enabled = true;
        }
        public void DisableColorButtons()
        {
            redButton.Enabled = false;
            greenButton.Enabled = false;
            yellowButton.Enabled = false;
            blueButton.Enabled = false;
        }
        
        private void CardButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Card selectedCard = clickedButton.Tag as Card;

            if (GameManager.Instance.IsValidMove(selectedCard))
            {
                if (selectedCard.Color == "Wild")
                {
                    EnableColorButtons();
                }
                else
                {
                    ClientSocket.SendData(new Message(MessageType.DanhBai, new List<string> { GameManager.Instance.Players[0].Name, (GameManager.Instance.Players[0].Hand.Count - 1).ToString(), selectedCard.CardName, selectedCard.Color }));
                }
                GameManager.Instance.CurrentCard = selectedCard;
                GameManager.Instance.Players[0].Hand.Remove(selectedCard);
                UpdateCurrentCardDisplay(selectedCard);
                PlayerHandPanel.Controls.Remove(clickedButton);
                DisableCardAndDrawButton();
            }
            else
            {
                MessageBox.Show("Invalid move.");
            }
        }
        private void RedButton_Click(object sender, EventArgs e)
        {
            GameManager.Instance.CurrentCard.Color = "Red";
            //Gửi thông điệp đến server theo định dạng DanhBai;ID;SoLuongBaiTrenTay;CardName;color
            ClientSocket.SendData(new Message(MessageType.DanhBai, new List<string> { GameManager.Instance.Players[0].Name, (GameManager.Instance.Players[0].Hand.Count).ToString(), GameManager.Instance.CurrentCard.CardName, GameManager.Instance.CurrentCard.Color }));
            DisableColorButtons();
            EnableCardAndDrawButton();
        }
        private void GreenButton_Click(object sender, EventArgs e)
        {
            GameManager.Instance.CurrentCard.Color = "Green";
            ClientSocket.SendData(new Message(MessageType.DanhBai, new List<string> { GameManager.Instance.Players[0].Name, (GameManager.Instance.Players[0].Hand.Count).ToString(), GameManager.Instance.CurrentCard.CardName, GameManager.Instance.CurrentCard.Color }));
            DisableColorButtons();
            DisableCardAndDrawButton();
        }
        private void YellowButton_Click(object sender, EventArgs e)
        {
            GameManager.Instance.CurrentCard.Color = "Yellow";
            ClientSocket.SendData(new Message(MessageType.DanhBai, new List<string> { GameManager.Instance.Players[0].Name, (GameManager.Instance.Players[0].Hand.Count).ToString(), GameManager.Instance.CurrentCard.CardName, GameManager.Instance.CurrentCard.Color }));
            DisableColorButtons();
            DisableCardAndDrawButton();
        }
        private void BlueButton_Click(object sender, EventArgs e)
        {
            GameManager.Instance.CurrentCard.Color = "Blue";
            ClientSocket.SendData(new Message(MessageType.DanhBai, new List<string> { GameManager.Instance.Players[0].Name, (GameManager.Instance.Players[0].Hand.Count).ToString(), GameManager.Instance.CurrentCard.CardName, GameManager.Instance.CurrentCard.Color }));
            DisableColorButtons();
            DisableCardAndDrawButton();
        }
        private void DrawCardButton_Click(object sender, EventArgs e)
        {
            ClientSocket.SendData(new Message(MessageType.RutBai, new List<string> { Program.player.Name, (GameManager.Instance.Players[0].Hand.Count + 1).ToString() }));
            // Update clientInfoLabel
            clientInfoLabel.Text = $"Tên: {Program.player.Name}";
            DisableColorButtons();
            DisableCardAndDrawButton();
        }

        public void EnableCardAndDrawButton()
        {
            // Enable the Draw Card button
            drawCardButton.Enabled = true;
            // Enable the Card buttons
            foreach (Button cardButton in PlayerHandPanel.Controls)
            {
                cardButton.Enabled = true;
            }
        }
        public void DisableCardAndDrawButton()
        {
            // Disable the Draw Card button
            drawCardButton.Enabled = false;
            // Disable the Card buttons
            foreach (Button cardButton in PlayerHandPanel.Controls)
            {
                cardButton.Enabled = false;
            }
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(945, 540);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        private Button drawCardButton;
        private ProgressBar turnTimer;
        private FlowLayoutPanel PlayerHandPanel;

        private PictureBox currentCardPictureBox;
        private Label currentColor;
        private Label currentPlayerLabel;
        private Label clientInfoLabel; // Nhãn để hiển thị tên và số bài của client

        private async void AnimateCardDrawing(Card card)
        {
            Button cardButton = new Button
            {
                Size = new Size(80, 120),
                BackgroundImage = GameResources.GetCardImage(card),
                BackgroundImageLayout = ImageLayout.Stretch,
                FlatStyle = FlatStyle.Flat,
                Tag = card,
                BackColor = Color.White,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.Black }
            };

            cardButton.FlatAppearance.MouseOverBackColor = Color.LightGray;
            cardButton.FlatAppearance.BorderSize = 1;

            Controls.Add(cardButton);

            Point startPoint = new Point(500, 110); // Starting point (deck location)
            Point endPoint = new Point(20 + (GameManager.Instance.Players[0].Hand.Count * 85), 60); // Ending point (player hand location)

            for (int i = 0; i <= 100; i += 5)
            {
                cardButton.Location = new Point(
                    startPoint.X + (endPoint.X - startPoint.X) * i / 100,
                    startPoint.Y + (endPoint.Y - startPoint.Y) * i / 100
                );
                await Task.Delay(10);
            }

            Controls.Remove(cardButton);
            DisplayPlayerHand(GameManager.Instance.Players[0].Hand);
        }

        private async void AnimateCardPlaying(Button cardButton, Card card)
        {
            Point startPoint = cardButton.Location; // Starting point (player hand location)
            Point endPoint = new Point(300, 50); // Ending point (center of the game board)

            for (int i = 0; i <= 100; i += 5)
            {
                cardButton.Location = new Point(
                    startPoint.X + (endPoint.X - startPoint.X) * i / 100,
                    startPoint.Y + (endPoint.Y - startPoint.Y) * i / 100
                );
                await Task.Delay(10);
            }

            PlayerHandPanel.Controls.Remove(cardButton);
            currentCard = card;
        }
        public static void YellUNOEnable()
        {
            // Assuming you have a reference to the Form1 instance
            Form1 form = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form != null)
            {
                form.Invoke(new Action(() => form.yellUNOButton.Enabled = true));
            }
        }

        private void InitializeCustomComponents()
        {
            // Initialize yellUNOButton
            yellUNOButton = new Button();
            yellUNOButton.Text = "Yell UNO!";
            yellUNOButton.Click += yellUNOButton_Click;
            // Add yellUNOButton to the form or a specific panel
            // Initialize other custom components if needed
        }

        public class CustomCardPanel : Panel
        {
            public CustomCardPanel()
            {
                this.AutoScroll = true; // Bật tính năng cuộn
            }
        }
        public void InitializeDeckImages()
        {
            var existingDeckImages = Controls.OfType<PictureBox>().Where(pb => pb.Tag != null && pb.Tag.ToString() == "DeckImage").ToList();
            var existingDeckLabels = Controls.OfType<Label>().Where(lbl => lbl.Tag != null && lbl.Tag.ToString() == "DeckLabel").ToList();
            foreach (var deckImageControl in existingDeckImages)
            {
                Controls.Remove(deckImageControl);
            }
            foreach (var deckLabelControl in existingDeckLabels)
            {
                Controls.Remove(deckLabelControl);
            }

            Image deckImage = Image.FromFile(@"Resources\CardImages\Deck.png");

            for (int i = 1; i < GameManager.Instance.Players.Count; i++) // Start from 1 to skip the current player
            {
                var player = GameManager.Instance.Players[i];
                PictureBox deckPictureBox = new PictureBox
                {
                    Size = new Size(100, 150),
                    Image = deckImage,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(this.ClientSize.Width - (120 + (i - 1) * 110), 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = "DeckImage"
                };

                Label deckLabel = new Label
                {
                    Size = new Size(100, 20),
                    Text = $"{player.Name}: {player.HandCount} cards",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(deckPictureBox.Left, deckPictureBox.Bottom + 5),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = Color.Transparent,
                    Tag = "DeckLabel"
                };

                Controls.Add(deckPictureBox);
                Controls.Add(deckLabel);
            }
            currentColor.Text = $"Màu hiện tại: {GameManager.Instance.CurrentCard.Color}";
        }

        private void InitializeChatPanel()
        {
            // Panel for chat
            chatPanel = new Panel
            {
                Size = new Size(250, currentCardPictureBox.Height), // Set the size of the chat panel
                Location = new Point(20, currentCardPictureBox.Top), // Align with currentCardPictureBox
                BackColor = Color.LightGray // Optional: Set the background color
            };

            // RichTextBox for chat history
            chatHistory = new RichTextBox
            {
                Dock = DockStyle.Top,
                Height = chatPanel.Height - 60, // Leave space for the input TextBox and Button
                ReadOnly = true,
                BackColor = Color.White
            };

            // TextBox for chat input
            chatInput = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 30
            };
            // Button to send chat message
            Button sendButton = new Button
            {
                Text = "Gửi",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            sendButton.Click += SendButton_Click;

            // Add controls to chat panel
            chatPanel.Controls.Add(chatHistory);
            chatPanel.Controls.Add(chatInput);
            chatPanel.Controls.Add(sendButton);

            // Add chat panel to form
            Controls.Add(chatPanel);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string message = chatInput.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                ClientSocket.SendData(new Message(MessageType.Chat, new List<string> { Program.player.Name, message }));
                AddChatMessage("You", message);
                chatInput.Clear();
            }
        }
        public static void UpdateOtherPlayerStat(Message message)
        {
            string[] data = message.Data.ToArray();
            string playerName = data[0];
            int turnOrder = int.Parse(data[1]);
            int cardCount = int.Parse(data[2]);

            Player player = GameManager.Instance.Players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                player = new Player(playerName);
                GameManager.Instance.Players.Add(player);
            }
            player.Hand = new List<Card>(new Card[cardCount]);

            Form1 form = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form != null)
            {
                form.Invoke(new Action(() =>
                {
                    form.InitializeDeckImages();
                }));
            }
        }
        // Helper classes

        // end aaasddd
    }
}