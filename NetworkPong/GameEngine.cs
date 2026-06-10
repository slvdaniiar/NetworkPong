namespace NetworkPong;

public class GameEngine
{
    private const int FieldWidth = 800;
    private const int FieldHeight = 450;
    private const int PaddleWidth = 14;
    private const int PaddleHeight = 90;
    private const int BallSize = 16;
    private const int PaddleSpeed = 8;
    private const int WinScore = 5;

    private int ballSpeedX = 6;
    private int ballSpeedY = 4;

    public GameState State { get; private set; }

    public GameEngine()
    {
        State = new GameState
        {
            BallX = FieldWidth / 2,
            BallY = FieldHeight / 2,
            Player1Y = FieldHeight / 2 - PaddleHeight / 2,
            Player2Y = FieldHeight / 2 - PaddleHeight / 2,
            Score1 = 0,
            Score2 = 0
        };
    }

    public void MovePlayer(int playerNumber, string direction)
    {
        if (State.Score1 >= WinScore || State.Score2 >= WinScore)
            return;

        if (playerNumber == 1)
        {
            if (direction == "UP") State.Player1Y -= PaddleSpeed;
            if (direction == "DOWN") State.Player1Y += PaddleSpeed;
            State.Player1Y = Math.Clamp(State.Player1Y, 0, FieldHeight - PaddleHeight);
        }
        else if (playerNumber == 2)
        {
            if (direction == "UP") State.Player2Y -= PaddleSpeed;
            if (direction == "DOWN") State.Player2Y += PaddleSpeed;
            State.Player2Y = Math.Clamp(State.Player2Y, 0, FieldHeight - PaddleHeight);
        }
    }

    public void Update()
    {
        if (State.Score1 >= WinScore || State.Score2 >= WinScore)
        {
            State.Status = State.Score1 > State.Score2 ? "Победил игрок 1" : "Победил игрок 2";
            return;
        }

        State.BallX += ballSpeedX;
        State.BallY += ballSpeedY;

        if (State.BallY <= 0 || State.BallY + BallSize >= FieldHeight)
            ballSpeedY = -ballSpeedY;

        CheckPaddleCollision();
        CheckGoal();
    }

    private void CheckPaddleCollision()
    {
        int leftPaddleX = 30;
        int rightPaddleX = FieldWidth - 44;

        bool hitLeft = State.BallX <= leftPaddleX + PaddleWidth &&
                       State.BallX + BallSize >= leftPaddleX &&
                       State.BallY + BallSize >= State.Player1Y &&
                       State.BallY <= State.Player1Y + PaddleHeight;

        bool hitRight = State.BallX + BallSize >= rightPaddleX &&
                        State.BallX <= rightPaddleX + PaddleWidth &&
                        State.BallY + BallSize >= State.Player2Y &&
                        State.BallY <= State.Player2Y + PaddleHeight;

        if (hitLeft && ballSpeedX < 0)
            ballSpeedX = -ballSpeedX;

        if (hitRight && ballSpeedX > 0)
            ballSpeedX = -ballSpeedX;
    }

    private void CheckGoal()
    {
        if (State.BallX < 0)
        {
            State.Score2++;
            ResetBall(-1);
        }
        else if (State.BallX > FieldWidth)
        {
            State.Score1++;
            ResetBall(1);
        }
    }

    private void ResetBall(int direction)
    {
        State.BallX = FieldWidth / 2;
        State.BallY = FieldHeight / 2;
        ballSpeedX = 6 * direction;
        ballSpeedY = 4;
    }
}
