namespace PicoControlApp
{
    partial class ControlForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlForm));
            this.textBox_server_ip = new System.Windows.Forms.TextBox();
            this.label_server_ip = new System.Windows.Forms.Label();
            this.checkBox_connect = new System.Windows.Forms.CheckBox();
            this.listBox_messages = new System.Windows.Forms.ListBox();
            this.textBox_message_input = new System.Windows.Forms.TextBox();
            this.button_send_message = new System.Windows.Forms.Button();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.groupBox_connection = new System.Windows.Forms.GroupBox();
            this.groupBox_messages = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_humidity = new System.Windows.Forms.TextBox();
            this.textBox_temperature = new System.Windows.Forms.TextBox();
            this.button_temperture_humidity = new System.Windows.Forms.Button();
            this.button_OFF = new System.Windows.Forms.Button();
            this.button_ON = new System.Windows.Forms.Button();
            this.button_initialize = new System.Windows.Forms.Button();
            this.button_status = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_right = new System.Windows.Forms.Button();
            this.button_left = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_speed = new System.Windows.Forms.TextBox();
            this.textBox_move_steps = new System.Windows.Forms.TextBox();
            this.label_move_steps = new System.Windows.Forms.Label();
            this.label_home_steps = new System.Windows.Forms.Label();
            this.textBox_home_steps = new System.Windows.Forms.TextBox();
            this.button_home = new System.Windows.Forms.Button();
            this.groupBox_connection.SuspendLayout();
            this.groupBox_messages.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_server_ip
            // 
            this.textBox_server_ip.Location = new System.Drawing.Point(72, 16);
            this.textBox_server_ip.Name = "textBox_server_ip";
            this.textBox_server_ip.Size = new System.Drawing.Size(106, 23);
            this.textBox_server_ip.TabIndex = 0;
            this.textBox_server_ip.Text = "192.168.30.4";
            // 
            // label_server_ip
            // 
            this.label_server_ip.AutoSize = true;
            this.label_server_ip.Location = new System.Drawing.Point(6, 19);
            this.label_server_ip.Name = "label_server_ip";
            this.label_server_ip.Size = new System.Drawing.Size(60, 15);
            this.label_server_ip.TabIndex = 1;
            this.label_server_ip.Text = "Server IP -";
            this.label_server_ip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBox_connect
            // 
            this.checkBox_connect.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_connect.Enabled = false;
            this.checkBox_connect.Location = new System.Drawing.Point(252, 16);
            this.checkBox_connect.Name = "checkBox_connect";
            this.checkBox_connect.Size = new System.Drawing.Size(90, 23);
            this.checkBox_connect.TabIndex = 2;
            this.checkBox_connect.Text = "Connect";
            this.checkBox_connect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_connect.UseVisualStyleBackColor = true;
            this.checkBox_connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // listBox_messages
            // 
            this.listBox_messages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_messages.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listBox_messages.FormattingEnabled = true;
            this.listBox_messages.ItemHeight = 14;
            this.listBox_messages.Location = new System.Drawing.Point(6, 39);
            this.listBox_messages.Name = "listBox_messages";
            this.listBox_messages.Size = new System.Drawing.Size(814, 382);
            this.listBox_messages.TabIndex = 3;
            // 
            // textBox_message_input
            // 
            this.textBox_message_input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_message_input.Location = new System.Drawing.Point(6, 12);
            this.textBox_message_input.Name = "textBox_message_input";
            this.textBox_message_input.Size = new System.Drawing.Size(751, 23);
            this.textBox_message_input.TabIndex = 4;
            this.textBox_message_input.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Message_Input_KeyPress);
            // 
            // button_send_message
            // 
            this.button_send_message.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_send_message.Location = new System.Drawing.Point(763, 12);
            this.button_send_message.Name = "button_send_message";
            this.button_send_message.Size = new System.Drawing.Size(57, 23);
            this.button_send_message.TabIndex = 5;
            this.button_send_message.Text = "Send";
            this.button_send_message.UseVisualStyleBackColor = true;
            this.button_send_message.Click += new System.EventHandler(this.Send_Message_Click);
            // 
            // textBox_port
            // 
            this.textBox_port.Enabled = false;
            this.textBox_port.Location = new System.Drawing.Point(184, 16);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(62, 23);
            this.textBox_port.TabIndex = 6;
            this.textBox_port.Text = "42440";
            // 
            // groupBox_connection
            // 
            this.groupBox_connection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox_connection.Controls.Add(this.checkBox_connect);
            this.groupBox_connection.Controls.Add(this.label_server_ip);
            this.groupBox_connection.Controls.Add(this.textBox_port);
            this.groupBox_connection.Controls.Add(this.textBox_server_ip);
            this.groupBox_connection.Location = new System.Drawing.Point(12, 449);
            this.groupBox_connection.Name = "groupBox_connection";
            this.groupBox_connection.Size = new System.Drawing.Size(351, 46);
            this.groupBox_connection.TabIndex = 8;
            this.groupBox_connection.TabStop = false;
            // 
            // groupBox_messages
            // 
            this.groupBox_messages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_messages.Controls.Add(this.textBox_message_input);
            this.groupBox_messages.Controls.Add(this.listBox_messages);
            this.groupBox_messages.Controls.Add(this.button_send_message);
            this.groupBox_messages.Location = new System.Drawing.Point(0, 0);
            this.groupBox_messages.Name = "groupBox_messages";
            this.groupBox_messages.Size = new System.Drawing.Size(826, 443);
            this.groupBox_messages.TabIndex = 9;
            this.groupBox_messages.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textBox_humidity);
            this.groupBox1.Controls.Add(this.textBox_temperature);
            this.groupBox1.Controls.Add(this.button_temperture_humidity);
            this.groupBox1.Controls.Add(this.button_OFF);
            this.groupBox1.Controls.Add(this.button_ON);
            this.groupBox1.Controls.Add(this.button_initialize);
            this.groupBox1.Controls.Add(this.button_status);
            this.groupBox1.Location = new System.Drawing.Point(6, 501);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 46);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // textBox_humidity
            // 
            this.textBox_humidity.Location = new System.Drawing.Point(367, 15);
            this.textBox_humidity.Name = "textBox_humidity";
            this.textBox_humidity.Size = new System.Drawing.Size(48, 23);
            this.textBox_humidity.TabIndex = 12;
            this.textBox_humidity.Text = "62.1";
            // 
            // textBox_temperature
            // 
            this.textBox_temperature.Location = new System.Drawing.Point(313, 15);
            this.textBox_temperature.Name = "textBox_temperature";
            this.textBox_temperature.Size = new System.Drawing.Size(48, 23);
            this.textBox_temperature.TabIndex = 11;
            this.textBox_temperature.Text = "25.52";
            // 
            // button_temperture_humidity
            // 
            this.button_temperture_humidity.Location = new System.Drawing.Point(253, 15);
            this.button_temperture_humidity.Name = "button_temperture_humidity";
            this.button_temperture_humidity.Size = new System.Drawing.Size(54, 23);
            this.button_temperture_humidity.TabIndex = 10;
            this.button_temperture_humidity.Text = "Meas";
            this.button_temperture_humidity.UseVisualStyleBackColor = true;
            this.button_temperture_humidity.Click += new System.EventHandler(this.Temperture_Humidity_Click);
            // 
            // button_OFF
            // 
            this.button_OFF.Location = new System.Drawing.Point(201, 15);
            this.button_OFF.Name = "button_OFF";
            this.button_OFF.Size = new System.Drawing.Size(46, 23);
            this.button_OFF.TabIndex = 9;
            this.button_OFF.Text = "OFF";
            this.button_OFF.UseVisualStyleBackColor = true;
            this.button_OFF.Click += new System.EventHandler(this.OFF_Click);
            // 
            // button_ON
            // 
            this.button_ON.Location = new System.Drawing.Point(149, 15);
            this.button_ON.Name = "button_ON";
            this.button_ON.Size = new System.Drawing.Size(46, 23);
            this.button_ON.TabIndex = 8;
            this.button_ON.Text = "ON";
            this.button_ON.UseVisualStyleBackColor = true;
            this.button_ON.Click += new System.EventHandler(this.ON_Click);
            // 
            // button_initialize
            // 
            this.button_initialize.Location = new System.Drawing.Point(72, 16);
            this.button_initialize.Name = "button_initialize";
            this.button_initialize.Size = new System.Drawing.Size(71, 23);
            this.button_initialize.TabIndex = 7;
            this.button_initialize.Text = "Initialize";
            this.button_initialize.UseVisualStyleBackColor = true;
            this.button_initialize.Click += new System.EventHandler(this.Initialize_Click);
            // 
            // button_status
            // 
            this.button_status.Location = new System.Drawing.Point(6, 16);
            this.button_status.Name = "button_status";
            this.button_status.Size = new System.Drawing.Size(60, 23);
            this.button_status.TabIndex = 6;
            this.button_status.Text = "Status";
            this.button_status.UseVisualStyleBackColor = true;
            this.button_status.Click += new System.EventHandler(this.Status_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.button_right);
            this.groupBox2.Controls.Add(this.button_left);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox_speed);
            this.groupBox2.Controls.Add(this.textBox_move_steps);
            this.groupBox2.Controls.Add(this.label_move_steps);
            this.groupBox2.Controls.Add(this.label_home_steps);
            this.groupBox2.Controls.Add(this.textBox_home_steps);
            this.groupBox2.Controls.Add(this.button_home);
            this.groupBox2.Location = new System.Drawing.Point(472, 449);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 76);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // button_right
            // 
            this.button_right.Location = new System.Drawing.Point(276, 44);
            this.button_right.Name = "button_right";
            this.button_right.Size = new System.Drawing.Size(60, 23);
            this.button_right.TabIndex = 18;
            this.button_right.Text = "Right";
            this.button_right.UseVisualStyleBackColor = true;
            this.button_right.Click += new System.EventHandler(this.MoveRight_Click);
            // 
            // button_left
            // 
            this.button_left.Location = new System.Drawing.Point(210, 44);
            this.button_left.Name = "button_left";
            this.button_left.Size = new System.Drawing.Size(60, 23);
            this.button_left.TabIndex = 17;
            this.button_left.Text = "Left";
            this.button_left.UseVisualStyleBackColor = true;
            this.button_left.Click += new System.EventHandler(this.MoveLeft_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "Speed -";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_speed
            // 
            this.textBox_speed.Location = new System.Drawing.Point(90, 44);
            this.textBox_speed.Name = "textBox_speed";
            this.textBox_speed.Size = new System.Drawing.Size(48, 23);
            this.textBox_speed.TabIndex = 15;
            this.textBox_speed.Text = "50";
            this.textBox_speed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.IntTextbox_KeyPress);
            // 
            // textBox_move_steps
            // 
            this.textBox_move_steps.Location = new System.Drawing.Point(225, 17);
            this.textBox_move_steps.Name = "textBox_move_steps";
            this.textBox_move_steps.Size = new System.Drawing.Size(48, 23);
            this.textBox_move_steps.TabIndex = 14;
            this.textBox_move_steps.Text = "200";
            this.textBox_move_steps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.IntTextbox_KeyPress);
            // 
            // label_move_steps
            // 
            this.label_move_steps.AutoSize = true;
            this.label_move_steps.Location = new System.Drawing.Point(144, 20);
            this.label_move_steps.Name = "label_move_steps";
            this.label_move_steps.Size = new System.Drawing.Size(75, 15);
            this.label_move_steps.TabIndex = 13;
            this.label_move_steps.Text = "Move steps -";
            this.label_move_steps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_home_steps
            // 
            this.label_home_steps.AutoSize = true;
            this.label_home_steps.Location = new System.Drawing.Point(6, 19);
            this.label_home_steps.Name = "label_home_steps";
            this.label_home_steps.Size = new System.Drawing.Size(78, 15);
            this.label_home_steps.TabIndex = 12;
            this.label_home_steps.Text = "Home steps -";
            this.label_home_steps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox_home_steps
            // 
            this.textBox_home_steps.Location = new System.Drawing.Point(90, 17);
            this.textBox_home_steps.Name = "textBox_home_steps";
            this.textBox_home_steps.Size = new System.Drawing.Size(48, 23);
            this.textBox_home_steps.TabIndex = 11;
            this.textBox_home_steps.Text = "300";
            this.textBox_home_steps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.IntTextbox_KeyPress);
            // 
            // button_home
            // 
            this.button_home.Location = new System.Drawing.Point(144, 44);
            this.button_home.Name = "button_home";
            this.button_home.Size = new System.Drawing.Size(60, 23);
            this.button_home.TabIndex = 6;
            this.button_home.Text = "Home";
            this.button_home.UseVisualStyleBackColor = true;
            this.button_home.Click += new System.EventHandler(this.MotorHome_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 570);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox_messages);
            this.Controls.Add(this.groupBox_connection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ControlForm";
            this.Text = "Raspbery Pi Pico Control";
            this.groupBox_connection.ResumeLayout(false);
            this.groupBox_connection.PerformLayout();
            this.groupBox_messages.ResumeLayout(false);
            this.groupBox_messages.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox textBox_server_ip;
        private Label label_server_ip;
        private CheckBox checkBox_connect;
        private ListBox listBox_messages;
        private TextBox textBox_message_input;
        private Button button_send_message;
        private TextBox textBox_port;
        private GroupBox groupBox_connection;
        private GroupBox groupBox_messages;
        private GroupBox groupBox1;
        private Button button_OFF;
        private Button button_ON;
        private Button button_initialize;
        private Button button_status;
        private TextBox textBox_humidity;
        private TextBox textBox_temperature;
        private Button button_temperture_humidity;
        private GroupBox groupBox2;
        private TextBox textBox_home_steps;
        private Button button_home;
        private Label label_home_steps;
        private Label label_move_steps;
        private Label label1;
        private TextBox textBox_speed;
        private TextBox textBox_move_steps;
        private Button button_left;
        private Button button_right;
    }
}