using System.Net.Sockets;
using System.Text;

namespace NetworkPong;

public class GameClient
{
    private TcpClient? client;
    private NetworkStream? stream;
    private bool isRunning;

    public event Action<GameState>? StateReceived;
    public event Action<string>? MessageReceived;

    public async Task ConnectAsync(string ip, int port)
    {
        client = new TcpClient();
        await client.ConnectAsync(ip, port);
        stream = client.GetStream();
        isRunning = true;
        MessageReceived?.Invoke("Подключение к серверу выполнено.");
        _ = Task.Run(ReadServerMessagesAsync);
    }

    public async Task SendMoveAsync(string direction)
    {
        if (stream == null) return;

        try
        {
            string message = $"MOVE|{direction}\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data);
        }
        catch
        {
            MessageReceived?.Invoke("Ошибка отправки данных.");
        }
    }

    private async Task ReadServerMessagesAsync()
    {
        byte[] buffer = new byte[1024];

        while (isRunning && stream != null)
        {
            try
            {
                int count = await stream.ReadAsync(buffer);
                if (count == 0) break;

                string text = Encoding.UTF8.GetString(buffer, 0, count).Trim();
                string[] messages = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (string message in messages)
                {
                    if (GameState.TryParse(message.Trim(), out GameState state))
                        StateReceived?.Invoke(state);
                }
            }
            catch
            {
                break;
            }
        }

        MessageReceived?.Invoke("Соединение с сервером потеряно.");
        Disconnect();
    }

    public void Disconnect()
    {
        isRunning = false;
        stream?.Close();
        client?.Close();
    }
}
