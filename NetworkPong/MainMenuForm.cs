namespace NetworkPong;

public class MainMenuForm : Form
{
    private readonly TextBox ipTextBox = new();
    private readonly TextBox portTextBox = new();

    public MainMenuForm()
    {
        Text = "Сетевой Пинг-Понг";
        Width = 420;
        Height = 320;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        Label title = new()
        {
            Text = "СЕТЕВОЙ ПИНГ-ПОНГ",
            Font = new Font("Times New Roman", 20, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Left = 20,
            Top = 25,
            Width = 360,
            Height = 45
        };

        Label ipLabel = new() { Text = "IP сервера:", Left = 70, Top = 95, Width = 100 };
        ipTextBox.Left = 170;
        ipTextBox.Top = 90;
        ipTextBox.Width = 150;
        ipTextBox.Text = "127.0.0.1";

        Label portLabel = new() { Text = "Порт:", Left = 70, Top = 130, Width = 100 };
        portTextBox.Left = 170;
        portTextBox.Top = 125;
        portTextBox.Width = 150;
        portTextBox.Text = "5000";

        Button serverButton = new()
        {
            Text = "Создать игру",
            Left = 100,
            Top = 170,
            Width = 200,
            Height = 35
        };
        serverButton.Click += ServerButton_Click;

        Button clientButton = new()
        {
            Text = "Подключиться",
            Left = 100,
            Top = 215,
            Width = 200,
            Height = 35
        };
        clientButton.Click += ClientButton_Click;

        Controls.Add(title);
        Controls.Add(ipLabel);
        Controls.Add(ipTextBox);
        Controls.Add(portLabel);
        Controls.Add(portTextBox);
        Controls.Add(serverButton);
        Controls.Add(clientButton);
    }

    private void ServerButton_Click(object? sender, EventArgs e)
    {
        int port = int.Parse(portTextBox.Text);
        GameForm gameForm = new(true, "127.0.0.1", port);
        gameForm.Show();
        Hide();
    }

    private void ClientButton_Click(object? sender, EventArgs e)
    {
        int port = int.Parse(portTextBox.Text);
        GameForm gameForm = new(false, ipTextBox.Text, port);
        gameForm.Show();
        Hide();
    }
}
