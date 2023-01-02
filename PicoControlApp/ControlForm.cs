using Shirehorse.Core.Extensions;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using PicoController;

namespace PicoControlApp
{
    public partial class ControlForm : Form
    {
        public ControlForm()
        {
            InitializeComponent();
            InitializeLoggingHandler();
            InitializeClient();
            Connect_Click(this, EventArgs.Empty);
        }

        public int MessageHistoryLength { get; set; } = 100;
        private readonly PicoBoilerInterface _boilerController = new();
        
        private void InitializeLoggingHandler()
        {
            Logger.NewMessage += (s, msg) => this.ThreadSafe(() =>
            {
                AddMessage($"{msg.Level}", msg.Message);
            });
        }

        private void InitializeClient()
        {
            _boilerController.ConnectionChanged += (s, connected) => this.ThreadSafe(() =>
            {
                checkBox_connect.Enabled = true;
                checkBox_connect.Checked = connected;
                checkBox_connect.Text = connected ? "Disconnect" : "Connect";
            });

            _boilerController.StatusReceived += (s, e) =>
            {
                Logger.Info(
                    $"DeviceState: {_boilerController.DeviceState}, " +
                    $"BoilerEnabled: {_boilerController.BoilerEnabled}, " +
                    $"Temperature: {_boilerController.Temperature}, " +
                    $"Humidity; {_boilerController.Humidity}");
            };

            _boilerController.Initiaize();
        }

        private void Message_Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (_boilerController.Connected)
                {
                    SendMessage(textBox_message_input.Text);
                    textBox_message_input.Clear();
                    e.Handled = true;
                }
                else
                {
                    AddMessage("Error", $"client not connected cannot send: {textBox_message_input.Text}");
                }
            }
        }

        private void SendMessage(string message) => _boilerController.SendMessage(message);

        private void AddMessage(string prefix, string message)
        {
            listBox_messages.Items.Insert(0, $"{prefix[..5],-6}| {message}");
            while (listBox_messages.Items.Count > MessageHistoryLength) listBox_messages.Items.RemoveAt(listBox_messages.Items.Count - 1);
        }

        private void Connect_Click(object sender, EventArgs e)
        {

            //checkBox_connect.Enabled = false;
            //checkBox_connect.Text = checkBox_connect.Checked ? "Connecting.." : "Disconnecting..";

            //if (int.TryParse(textBox_port.Text, out int port))
            //{
            //    _boilerController.Port = port;
            //}
            //else
            //{
            //    Logger.Warning($"failed to parse\"{textBox_port.Text}\" as integer");
            //}
            //_boilerController.Connect();
        }

        private void Send_Message_Click(object sender, EventArgs e) => SendMessage(textBox_message_input.Text);
        private void Status_Click(object sender, EventArgs e) => _boilerController.RequestStatus();
        private void Initialize_Click(object sender, EventArgs e) => _boilerController.InitializeDevice();
        private void ON_Click(object sender, EventArgs e) => _boilerController.SetBoilerEnabled(true);
        private void OFF_Click(object sender, EventArgs e) => _boilerController.SetBoilerEnabled(false);
        private void Temperture_Humidity_Click(object sender, EventArgs e)
        {
            try
            {
                float temperature = float.Parse(textBox_temperature.Text);
                float humidity = float.Parse(textBox_humidity.Text);

                _boilerController.SendMeasurment(temperature, humidity);
            }
            catch (Exception ex)
            {
                Logger.Error($"failed to send measurement: {ex.Message}");
                Logger.Debug(ex);
            }
        }

        private void MotorHome_Click(object sender, EventArgs e) => SendMessage($"move|-{textBox_home_steps.Text}");
        private void MoveLeft_Click(object sender, EventArgs e) => SendMessage($"move|-{textBox_move_steps.Text}");
        private void MoveRight_Click(object sender, EventArgs e) => SendMessage($"move|{textBox_move_steps.Text}");
        private void IntTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && sender is TextBox textbox)
            {
                if (int.TryParse(textbox.Text, out int value))
                {
                    string? parmeterKey = textbox.Name switch
                    {
                        "textBox_home_steps" => "motor_home_steps",
                        "textBox_move_steps" => "motor_boiler_enable_steps",
                        "textBox_speed" => "motor_speed",
                        _ => null,
                    };

                    if (parmeterKey is not null)
                    {
                        _boilerController.SendParameters(new() { { parmeterKey, $"{value}" } });
                    }
                }
                else
                {
                    Logger.Error($"could not parse as int: {textBox_speed.Text}");
                }
            }
        }

        
    }
}