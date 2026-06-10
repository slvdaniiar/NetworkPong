namespace NetworkPong;

public class GameState
{
    public int BallX { get; set; }
    public int BallY { get; set; }
    public int Player1Y { get; set; }
    public int Player2Y { get; set; }
    public int Score1 { get; set; }
    public int Score2 { get; set; }
    public string Status { get; set; } = "Игра идет";

    public string ToNetworkMessage()
    {
        return $"STATE|{BallX}|{BallY}|{Player1Y}|{Player2Y}|{Score1}|{Score2}|{Status}";
    }

    public static bool TryParse(string message, out GameState state)
    {
        state = new GameState();
        string[] parts = message.Split('|');

        if (parts.Length < 8 || parts[0] != "STATE")
            return false;

        if (!int.TryParse(parts[1], out int ballX)) return false;
        if (!int.TryParse(parts[2], out int ballY)) return false;
        if (!int.TryParse(parts[3], out int player1Y)) return false;
        if (!int.TryParse(parts[4], out int player2Y)) return false;
        if (!int.TryParse(parts[5], out int score1)) return false;
        if (!int.TryParse(parts[6], out int score2)) return false;

        state.BallX = ballX;
        state.BallY = ballY;
        state.Player1Y = player1Y;
        state.Player2Y = player2Y;
        state.Score1 = score1;
        state.Score2 = score2;
        state.Status = parts[7];

        return true;
    }
}
