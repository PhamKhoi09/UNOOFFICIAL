using System.Collections.Generic;

public class Player
{
    public string Name { get; set; }
    public List<Card> Hand { get; set; }
    public int HandCount { get; set; }
    public bool IsTurn { get; set; }
    public int Points { get; internal set; }
    public int Rank { get; internal set; }

    public Player() // Constructor mặc định 
    {
        Hand = new List<Card>();
    }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
        IsTurn = false;
    }
}