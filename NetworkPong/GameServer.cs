using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkPong;

public class GameServer
{
    private TcpListener? listener;
    private TcpClient? player2Client;
    private NetworkStream? player2Stream;
    private readonly GameEngine engine = new();
    private bool isRunning;

    public event Action<GameState>? StateChanged;
    public event Action<string>? MessageReceived;

    public async Task StartAsync(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        isRunning = true;

        MessageReceived?.Invoke("Сервер запущен. Ожидание второго игрока...");
        player2Client = await listener.AcceptTcpClientAsync();
        player2Stream = player2Client.GetStream();
        MessageReceived?.Invoke("Второй игрок подключился.");

        _ = Task.Run(ReadClientCommandsAsync);
        _ = Task.Run(GameLoopAsync);
    }

    public void MoveHostPlayer(string direction)
    {
        engine.MovePlayer(1, direction);
    }

    private async Task ReadClientCommandsAsync()
    {
        byte[] buffer = new byte[1024];

        while (isRunning && player2Stream != null)
        {
            try
            {
                int count = await player2Stream.ReadAsync(buffer);
                if (count == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, count).Trim();
                string[] commands = message.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (string command in commands)
                {
                    string[] parts = command.Trim().Split('|');
                    if (parts.Length == 2 && parts[0] == "MOVE")
                        engine.MovePlayer(2, parts[1]);
                }
            }
            catch
            {
                break;
            }
        }

        MessageReceived?.Invoke("Клиент отключился.");
        Stop();
    }

    private async Task GameLoopAsync()
    {
        while (isRunning)
        {
            engine.Update();
            GameState state = engine.State;
            StateChanged?.Invoke(state);
            await SendStateToClientAsync(state);
            await Task.Delay(20);
        }
    }

    private async Task SendStateToClientAsync(GameState state)
    {
        if (player2Stream == null) return;

        try
        {
            string message = state.ToNetworkMessage() + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await player2Stream.WriteAsync(data);
        }
        catch
        {
            Stop();
        }
    }

    public void Stop()
    {
        isRunning = false;
        player2Stream?.Close();
        player2Client?.Close();
        listener?.Stop();
    }
}
