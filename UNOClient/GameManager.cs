using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UnoOnline
{
    public class GameManager
    {
        private static readonly object lockObject = new object();
        private static GameManager instance;

        public List<Player> Players { get; set; }
        public Card CurrentCard { get; set; }
        public int CurrentPlayerIndex { get; set; }
        public bool IsSpecialDraw { get; set; }
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new GameManager();
                        }
                    }
                }
                return instance;
            }
        }
        private GameManager()
        {
            Players = new List<Player>();
            CurrentCard = new Card();
            CurrentPlayerIndex = 0;
            IsSpecialDraw = false;
        }

        public void UpdateOtherPlayerName(string otherPlayerName)
        {
            lock (lockObject)
            {
                if (Players == null)
                {
                    Players = new List<Player>();
                }

                bool playerExists = Players.Exists(p => p.Name == otherPlayerName);
                if (!playerExists)
                {
                    Players.Add(new Player(otherPlayerName));
                }
            }

            WaitingLobby form = (WaitingLobby)Application.OpenForms.OfType<WaitingLobby>().FirstOrDefault();
            if (form != null)
            {
                form.Invoke(new Action(() =>
                {
                    form.UpdatePlayerList(Players);
                }));
            }
        }

        public static void InitializeStat(Message message)
        {
            if (Instance == null)
            {
                instance = new GameManager();
            }

            string[] data = message.Data.ToArray();
            string playerName = data[0];
            int turnOrder = int.Parse(data[1]);
            int cardCount = int.Parse(data[2]);

            // Lấy danh sách các lá bài
            List<string> cardNames = new List<string>(data.Skip(3).Take(cardCount));

            // Thêm các lá bài vào tay người chơi
            Instance.Players[0].Hand = cardNames.Select(cardData =>
            {
                string[] card = cardData.Split('_');
                string color = card[0];
                string value = card[1];
                if (color == "Wild" && value != "Draw")
                {
                    value = "Wild";
                }
                return new Card(cardData, color, value);
            }).ToList();

            string currentCardName = data[10];
            string[] currentCard = data[10].Split('_');
            string currentColor = currentCard[0];
            string currentValue = currentCard[1];
            Instance.CurrentCard = new Card(currentCardName, currentColor, currentValue);
        }

        public static void UpdateOtherPlayerStat(Message message)
        {
            //ID;Lượt;Số lượng bài
            string[] data = message.Data.ToArray();
            string playerName = data[0];
            int turnOrder = int.Parse(data[1]);
            int cardCount = int.Parse(data[2]);

            Player player = Instance.Players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                player = new Player(playerName);
                Instance.Players.Add(player);
            }
            player.HandCount = cardCount;

        }


        public static void Boot()
        {
            //Mở màn hình game mở (nếu chưa)
            if (!Program.IsFormOpen(typeof(Form1)))
            {
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    if (!Program.IsFormOpen(typeof(Form1)))
                    {
                        Form1 form1 = new Form1();
                        form1.Show();
                        form1.DisplayPlayerHand(Instance.Players[0].Hand);
                        form1.UpdateCurrentCardDisplay(Instance.CurrentCard);

                        form1.InitializeDeckImages();
                        form1.DisableCardAndDrawButton();
                        form1.DisableColorButtons();
                    }
                }));
            }
            //đóng màn hình win lose, nếu có
            if (Program.IsFormOpen(typeof(WinResult)))
            {
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    WinResult winResult = (WinResult)Application.OpenForms[typeof(WinResult).Name];
                    winResult.Close();
                }));
            }
            else if (Program.IsFormOpen(typeof(LoseResult)))
            {
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    LoseResult loseResult = (LoseResult)Application.OpenForms[typeof(LoseResult).Name];
                    loseResult.Close();
                }));
            }
        }

        public bool IsValidMove(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card), "Card cannot be null");
            }
            return card.Color == Instance.CurrentCard.Color || card.Value == Instance.CurrentCard.Value || card.Color == "Wild" ;
        }

        public void HandleUpdate(Message message)
        {
            try
            {
                string[] data = message.Data.ToArray();
                // Message received: Update; ID; RemainingCards; CardName (if a card is played); color (if the card is a wild card)
                if (data.Length < 2)
                {
                    throw new ArgumentException("Invalid card data: not enough elements.");
                }
                //Update;anle;7
                string playerId = data[0];
                if (!int.TryParse(data[1], out int remainingCards))
                {
                    throw new FormatException("Input string was not in a correct format: " + data[1]); //7Turn
                }

                // Find the player in the list
                Player player = Instance.Players.FirstOrDefault(p => p.Name == playerId);
                if (player != null)
                {
                    player.HandCount = remainingCards;
                }

                if (playerId != Program.player.Name)
                {
                    Instance.IsSpecialDraw = false;
                    // If another player has played a card
                    if (data.Length == 3)
                    {
                        if (Instance.CurrentCard == null)
                        {
                            Instance.CurrentCard = new Card();
                        }
                        Instance.CurrentCard.CardName = data[2];
                        string[] card = data[2].Split('_');
                        if (card.Length < 2)
                        {
                            throw new ArgumentException($"Invalid card data: {data[2]}");
                        }
                        Instance.CurrentCard.Color = card[0];
                        Instance.CurrentCard.Value = card[1];
                        if (Instance.CurrentCard.CardName.Contains("Draw"))
                        {
                            Instance.IsSpecialDraw = true;
                        }
                        else
                        {
                            Instance.IsSpecialDraw = false;
                        }
                    }
                    // If the card is a wild card or draw 4
                    else if (data.Length == 4)
                    {
                        if (Instance.CurrentCard == null)
                        {
                            Instance.CurrentCard = new Card();
                        }
                        Instance.CurrentCard.CardName = data[2];
                        string[] card = data[2].Split('_');
                        if (card.Length < 2)
                        {
                            throw new ArgumentException($"Invalid card data: {data[2]}");
                        }
                        Instance.CurrentCard.Value = card[1];
                        Instance.CurrentCard.Color = data[3];
                        if (Instance.CurrentCard.CardName.Contains("Draw"))
                        {
                            Instance.IsSpecialDraw = true;
                        }
                        else
                        {
                            Instance.IsSpecialDraw = false;
                        }
                    }
                }
                // Update the UI
                Form1 form1 = (Form1)Application.OpenForms.OfType<Form1>().FirstOrDefault();
                if (form1 != null)
                {
                    form1.Invoke(new Action(() =>
                    {
                        if (Instance.CurrentCard != null)
                        {
                            form1.UpdateCurrentCardDisplay(Instance.CurrentCard);
                        }
                        else
                        {
                            MessageBox.Show("CurrentCard is null.");
                        }
                        form1.InitializeDeckImages(); // Refresh the deck images and labels
                    }));
                }
                else
                {
                    MessageBox.Show("Form1 is null.");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Input string was not in a correct format: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Invalid card data: " + ex.Message);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Object not initialized: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message);
            }
        }

        public static void HandleTurnMessage(Message message)
        {
            try
            {
                string playerId = message.Data[0];
                Form1.UpdateCurrentPlayerLabel(playerId);
                if (playerId == Program.player.Name)
                {
                    //MessageBox.Show("It's the current player's turn.");

                    if (Instance.CurrentCard.CardName.Contains("Draw") && Instance.IsSpecialDraw == true) //Bị rút bài
                    {
                        Instance.IsSpecialDraw = false;
                        if (Instance.CurrentCard.CardName.Contains("Wild"))
                            // Draw 4
                            ClientSocket.SendData(new Message(MessageType.SpecialCardEffect, new List<string> { Program.player.Name, (Instance.Players[0].Hand.Count + 4).ToString() }));
                        else
                            // Draw 2
                            ClientSocket.SendData(new Message(MessageType.SpecialCardEffect, new List<string> { Program.player.Name, (Instance.Players[0].Hand.Count + 2).ToString() }));
                    }
                    else
                    {
                        //Enable EnableCardAndDrawButton on form1
                        Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                        if (form1 != null)
                        {
                            form1.Invoke(new Action(() =>
                            {
                                form1.EnableCardAndDrawButton();
                            }));
                        }
                        else
                        {
                            MessageBox.Show("Form1 is null.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in HandleTurnMessage: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public static void HandleSpecialDraw(Message message)
        {
            try
            {
                // Example message data: "Specialdraws;anle;Yellow_Skip_;Green_Draw;Green_7;Red_3;"
                string[] data = message.Data.Where(d => !string.IsNullOrEmpty(d)).ToArray();
                if (data.Length < 2)
                {
                    throw new ArgumentException("Invalid message data: not enough elements.");
                }

                List<string> cardNames = new List<string>(data.Skip(1)); // Skip the first part which is not card data

                Instance.Players[0].Hand.AddRange(cardNames.Select(cardData =>
                {
                    string[] card = cardData.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (card.Length < 2)
                    {
                        throw new ArgumentException($"Invalid card data: {cardData}");
                    }
                    string color = card[0];
                    string value = card[1];
                    if (color == "Wild" && value != "Draw")
                    {
                        value = "Wild";
                    }
                    return new Card(cardData, color, value);
                }).ToList());

                //Hiển thị bài trên tay
                Form1 form1 = (Form1)Application.OpenForms.OfType<Form1>().FirstOrDefault();
                if (form1 != null)
                {
                    form1.Invoke(new Action(() =>
                    {
                        form1.DisplayPlayerHand(Instance.Players[0].Hand);
                        form1.InitializeDeckImages();
                        form1.DisableCardAndDrawButton();
                    }));
                }
                else
                {
                    MessageBox.Show("Form1 is null.");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Invalid card data: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message);
            }
        }

        public static void HandleCardDraw(Message message)
        {
            string playerName = message.Data[0];
            string cardName = message.Data[1];
            string[] card = cardName.Split('_');
            string color = card[0];
            string value = card[1];
            Instance.Players[0].Hand.Add(new Card(cardName, color, value));

            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 != null)
            {
                form1.Invoke(new Action(() =>
                {
                    form1.DisplayPlayerHand(Instance.Players[0].Hand);
                    form1.InitializeDeckImages();
                    form1.DisableCardAndDrawButton();
                }));
            }
        }

        public static void Penalty(Message message)
        {
            string playerGotPenalty = message.Data[0];
            if (playerGotPenalty == Program.player.Name)
            {
                MessageBox.Show("You got penalty for not pressing the UNO button!");
                if (Instance.CurrentCard.CardName.Contains("Draw") && Instance.IsSpecialDraw == true) //Bị rút bài
                {
                    Instance.IsSpecialDraw = false;
                    if (Instance.CurrentCard.CardName.Contains("Wild"))
                        // Draw 4
                        ClientSocket.SendData(new Message(MessageType.DrawPenalty, new List<string> { Program.player.Name, (Instance.Players[0].Hand.Count + 6).ToString() }));
                    else
                        // Draw 2
                        ClientSocket.SendData(new Message(MessageType.DrawPenalty, new List<string> { Program.player.Name, (Instance.Players[0].Hand.Count + 4).ToString() }));
                }
                else ClientSocket.SendData(new Message(MessageType.DrawPenalty, new List<string> { Program.player.Name, (Instance.Players[0].Hand.Count + 2).ToString() }));
            }
        }

        public static void HandleChatMessage(Message message)
        {
            string playerName = message.Data[0];
            string chatMessage = message.Data[1];
            //Hiển thị lên form1 AddChatMessage
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 != null)
            {
                form1.Invoke(new Action(() =>
                {
                    form1.AddChatMessage(playerName, chatMessage);
                }));
            }
            else
            {
                MessageBox.Show("Form1 is null.");
            }
        }
        public static void HandleNotEnoughPlayer(Message message)
        {
            //Hiển thị thông báo không đủ người chơi nếu form1 chưa bật
            //Hiển thị thông báo kết thúc game vì có người chơi thoát đột ngột (và số người chơi còn lại ít hơn 2)
            if(!Program.IsFormOpen(typeof(Form1)))
            {
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    MessageBox.Show("Not enough players to start the game.");
                }));
            }
            else
            {
                //Hiển thị Game ended because a player left the game. và tắt form
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    MessageBox.Show("Game ended because a player left the game.");
                    Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                    if (form1 != null)
                    {
                        form1.Invoke(new Action(() =>
                        {
                            form1.Close();
                        }));
                    }
                }));
                //tắt waiting lobby
                WaitingLobby waitingLobby = Application.OpenForms.OfType<WaitingLobby>().FirstOrDefault();
                if (waitingLobby != null)
                {
                    waitingLobby.Invoke(new Action(() =>
                    {
                        waitingLobby.Close();
                    }));
                }
            }
        }
        public static void HandleEndMessage(Message message)
        {
            string[] data = message.Data.ToArray();
            string winnerName = data[0];
            int PenaltyPoint = Instance.Players[0].Hand.Count * 10;
            if (winnerName == Program.player.Name)
            {
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    WinResult winResult = new WinResult();
                    winResult.Show();
                }));
            }
            else
            {
                ClientSocket.SendData(new Message(MessageType.Diem, new List<string> { Program.player.Name, PenaltyPoint.ToString() }));
                Application.OpenForms[0].Invoke(new Action(() =>
                {
                    LoseResult loseResult = new LoseResult();
                    loseResult.Show();
                }));
            }
            
            //Clear players hand
            Instance.Players[0].Hand.Clear();

            //Close Form1
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 != null)
            {
                form1.Invoke(new Action(() =>
                {
                    form1.Close();
                }));
            }
        }

        public static void HandleResult(Message message)
        {
            // Result;ID;Diem;Rank
            string playerId = message.Data[0];
            int points = int.Parse(message.Data[1]);
            int rank = int.Parse(message.Data[2]);
            Player player = Instance.Players.FirstOrDefault(p => p.Name == playerId);
            if (player != null)
            {
                player.Points = points;
                player.Rank = rank;
            }
            Application.OpenForms[0].Invoke(new Action(() =>
            {
                FinalRanking finalRanking = FinalRanking.Instance;
                finalRanking.DisplayRanking(GameManager.Instance.Players);
                finalRanking.Show();

                //Tắt winResult hoặc loseResult nếu có
                if (Program.IsFormOpen(typeof(WinResult)))
                {
                    WinResult winResult = (WinResult)Application.OpenForms[typeof(WinResult).Name];
                    winResult.Close();
                }
                else if (Program.IsFormOpen(typeof(LoseResult)))
                {
                    LoseResult loseResult = (LoseResult)Application.OpenForms[typeof(LoseResult).Name];
                    loseResult.Close();
                }
            }));

        }
    }
}
