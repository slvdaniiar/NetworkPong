namespace NetworkPong;

public class GameForm : Form
{
    private const int FieldWidth = 800;
    private const int FieldHeight = 450;
    private const int PaddleWidth = 14;
    private const int PaddleHeight = 90;
    private const int BallSize = 16;

    private readonly bool isServer;
    private readonly string ip;
    private readonly int port;

    private GameServer? server;
    private GameClient? client;
    private GameState state = new();

    private bool upPressed;
    private bool downPressed;

    private readonly System.Windows.Forms.Timer inputTimer = new();

    public GameForm(bool isServer, string ip, int port)
    {
        this.isServer = isServer;
        this.ip = ip;
        this.port = port;

        Text = isServer ? "Пинг-Понг: Игрок 1 (сервер)" : "Пинг-Понг: Игрок 2 (клиент)";
        Width = FieldWidth + 16;
        Height = FieldHeight + 39;
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;
        BackColor = Color.Black;
        KeyPreview = true;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        KeyDown += GameForm_KeyDown;
        KeyUp += GameForm_KeyUp;
        FormClosing += GameForm_FormClosing;
        Load += GameForm_Load;

        inputTimer.Interval = 20;
        inputTimer.Tick += InputTimer_Tick;
        inputTimer.Start();
    }

    private async void GameForm_Load(object? sender, EventArgs e)
    {
        if (isServer)
        {
            server = new GameServer();
            server.StateChanged += Server_StateChanged;
            server.MessageReceived += ShowInfo;
            await server.StartAsync(port);
        }
        else
        {
            client = new GameClient();
            client.StateReceived += Client_StateReceived;
            client.MessageReceived += ShowInfo;
            await client.ConnectAsync(ip, port);
        }
    }

    private void Server_StateChanged(GameState newState)
    {
        state = newState;
        BeginInvoke(new Action(Invalidate));
    }

    private void Client_StateReceived(GameState newState)
    {
        state = newState;
        BeginInvoke(new Action(Invalidate));
    }

    private void ShowInfo(string text)
    {
        if (!IsDisposed)
            BeginInvoke(new Action(() => Text = text));
    }

    private async void InputTimer_Tick(object? sender, EventArgs e)
    {
        if (upPressed)
            await SendMove("UP");

        if (downPressed)
            await SendMove("DOWN");
    }

    private async Task SendMove(string direction)
    {
        if (isServer)
            server?.MoveHostPlayer(direction);
        else if (client != null)
            await client.SendMoveAsync(direction);
    }

    private void GameForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            upPressed = true;

        if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            downPressed = true;
    }

    private void GameForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            upPressed = false;

        if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            downPressed = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        using Pen centerPen = new(Color.Gray, 2);
        for (int y = 0; y < FieldHeight; y += 30)
            g.DrawLine(centerPen, FieldWidth / 2, y, FieldWidth / 2, y + 15);

        using Brush leftBrush = new SolidBrush(Color.DeepSkyBlue);
        using Brush rightBrush = new SolidBrush(Color.Orange);
        using Brush ballBrush = new SolidBrush(Color.White);
        using Brush textBrush = new SolidBrush(Color.White);
        using Font scoreFont = new("Times New Roman", 24, FontStyle.Bold);
        using Font statusFont = new("Times New Roman", 16, FontStyle.Bold);

        g.FillRectangle(leftBrush, 30, state.Player1Y, PaddleWidth, PaddleHeight);
        g.FillRectangle(rightBrush, FieldWidth - 44, state.Player2Y, PaddleWidth, PaddleHeight);
        g.FillEllipse(ballBrush, state.BallX, state.BallY, BallSize, BallSize);

        string score = $"{state.Score1} : {state.Score2}";
        SizeF scoreSize = g.MeasureString(score, scoreFont);
        g.DrawString(score, scoreFont, textBrush, FieldWidth / 2 - scoreSize.Width / 2, 20);

        if (!string.IsNullOrWhiteSpace(state.Status) && state.Status != "Игра идет")
        {
            SizeF statusSize = g.MeasureString(state.Status, statusFont);
            g.DrawString(state.Status, statusFont, textBrush, FieldWidth / 2 - statusSize.Width / 2, 70);
        }
    }

    private void GameForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        inputTimer.Stop();
        server?.Stop();
        client?.Disconnect();
        Application.Exit();
    }
}
