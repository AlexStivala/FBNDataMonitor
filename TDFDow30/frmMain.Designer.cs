namespace TDFDow30
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblHostName = new System.Windows.Forms.Label();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ServerTextBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblLogResp = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.PWlabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PWTextBox = new System.Windows.Forms.TextBox();
            this.Userlabel = new System.Windows.Forms.Label();
            this.UserTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.IPlabel = new System.Windows.Forms.Label();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.DataResetLabel = new System.Windows.Forms.Label();
            this.gbTimeOfLastDelete = new System.Windows.Forms.GroupBox();
            this.lblReceivedTime = new System.Windows.Forms.Label();
            this.gbTime = new System.Windows.Forms.GroupBox();
            this.timeOfDayLabel = new System.Windows.Forms.Label();
            this.symbolDataGrid = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button4 = new System.Windows.Forms.Button();
            this.TODTimer = new System.Windows.Forms.Timer(this.components);
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.WatchdogTimer = new System.Windows.Forms.Timer(this.components);
            this.ResetTimer = new System.Windows.Forms.Timer(this.components);
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.InitTimer = new System.Windows.Forms.Timer(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.ReconnectTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.DailyResetLabel = new System.Windows.Forms.Label();
            this.ServerResetLabel = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbTimeOfLastDelete.SuspendLayout();
            this.gbTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.symbolDataGrid)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblHostName);
            this.groupBox5.Controls.Add(this.lblIpAddress);
            this.groupBox5.Location = new System.Drawing.Point(10, 87);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox5.Size = new System.Drawing.Size(513, 58);
            this.groupBox5.TabIndex = 172;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Host PC Info";
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHostName.Location = new System.Drawing.Point(166, 25);
            this.lblHostName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(49, 25);
            this.lblHostName.TabIndex = 122;
            this.lblHostName.Text = "N/A";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIpAddress.Location = new System.Drawing.Point(9, 25);
            this.lblIpAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(49, 25);
            this.lblIpAddress.TabIndex = 121;
            this.lblIpAddress.Text = "N/A";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ServerTextBox);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.lblLogResp);
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.PWlabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.PWTextBox);
            this.groupBox1.Controls.Add(this.Userlabel);
            this.groupBox1.Controls.Add(this.UserTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.PortTextBox);
            this.groupBox1.Controls.Add(this.IPlabel);
            this.groupBox1.Controls.Add(this.IPTextBox);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 233);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(824, 131);
            this.groupBox1.TabIndex = 171;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login Info";
            // 
            // ServerTextBox
            // 
            this.ServerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerTextBox.Location = new System.Drawing.Point(26, 51);
            this.ServerTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ServerTextBox.Name = "ServerTextBox";
            this.ServerTextBox.ReadOnly = true;
            this.ServerTextBox.Size = new System.Drawing.Size(88, 30);
            this.ServerTextBox.TabIndex = 179;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(21, 22);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 25);
            this.label15.TabIndex = 178;
            this.label15.Text = "Server ID";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Location = new System.Drawing.Point(614, 19);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(148, 74);
            this.groupBox2.TabIndex = 177;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connected";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(64, 34);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(24, 25);
            this.pictureBox2.TabIndex = 164;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(64, 32);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 25);
            this.pictureBox1.TabIndex = 163;
            this.pictureBox1.TabStop = false;
            // 
            // lblLogResp
            // 
            this.lblLogResp.AutoSize = true;
            this.lblLogResp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogResp.Location = new System.Drawing.Point(10, 95);
            this.lblLogResp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogResp.Name = "lblLogResp";
            this.lblLogResp.Size = new System.Drawing.Size(181, 25);
            this.lblLogResp.TabIndex = 160;
            this.lblLogResp.Text = "Logon Response:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(1050, 33);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(158, 60);
            this.ConnectButton.TabIndex = 159;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Visible = false;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(1198, 43);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 25);
            this.label7.TabIndex = 167;
            this.label7.Text = "Num Catalogs: ";
            this.label7.Visible = false;
            // 
            // PWlabel
            // 
            this.PWlabel.AutoSize = true;
            this.PWlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PWlabel.Location = new System.Drawing.Point(580, 75);
            this.PWlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PWlabel.Name = "PWlabel";
            this.PWlabel.Size = new System.Drawing.Size(106, 25);
            this.PWlabel.TabIndex = 158;
            this.PWlabel.Text = "Password";
            this.PWlabel.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(1198, 68);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 25);
            this.label6.TabIndex = 166;
            this.label6.Text = "Num Fields: ";
            this.label6.Visible = false;
            // 
            // PWTextBox
            // 
            this.PWTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PWTextBox.Location = new System.Drawing.Point(584, 103);
            this.PWTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PWTextBox.Name = "PWTextBox";
            this.PWTextBox.ReadOnly = true;
            this.PWTextBox.Size = new System.Drawing.Size(102, 30);
            this.PWTextBox.TabIndex = 157;
            this.PWTextBox.Visible = false;
            // 
            // Userlabel
            // 
            this.Userlabel.AutoSize = true;
            this.Userlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Userlabel.Location = new System.Drawing.Point(408, 74);
            this.Userlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Userlabel.Name = "Userlabel";
            this.Userlabel.Size = new System.Drawing.Size(119, 25);
            this.Userlabel.TabIndex = 156;
            this.Userlabel.Text = "User Name";
            this.Userlabel.Visible = false;
            // 
            // UserTextBox
            // 
            this.UserTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserTextBox.Location = new System.Drawing.Point(413, 102);
            this.UserTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UserTextBox.Name = "UserTextBox";
            this.UserTextBox.ReadOnly = true;
            this.UserTextBox.Size = new System.Drawing.Size(125, 30);
            this.UserTextBox.TabIndex = 155;
            this.UserTextBox.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(332, 19);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 25);
            this.label4.TabIndex = 154;
            this.label4.Text = "Port";
            // 
            // PortTextBox
            // 
            this.PortTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortTextBox.Location = new System.Drawing.Point(337, 47);
            this.PortTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.ReadOnly = true;
            this.PortTextBox.Size = new System.Drawing.Size(88, 30);
            this.PortTextBox.TabIndex = 153;
            // 
            // IPlabel
            // 
            this.IPlabel.AutoSize = true;
            this.IPlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPlabel.Location = new System.Drawing.Point(157, 22);
            this.IPlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.IPlabel.Name = "IPlabel";
            this.IPlabel.Size = new System.Drawing.Size(118, 25);
            this.IPlabel.TabIndex = 152;
            this.IPlabel.Text = "IP Address";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPTextBox.Location = new System.Drawing.Point(154, 50);
            this.IPTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.ReadOnly = true;
            this.IPTextBox.Size = new System.Drawing.Size(148, 30);
            this.IPTextBox.TabIndex = 151;
            // 
            // DataResetLabel
            // 
            this.DataResetLabel.AutoSize = true;
            this.DataResetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DataResetLabel.Location = new System.Drawing.Point(13, 627);
            this.DataResetLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DataResetLabel.Name = "DataResetLabel";
            this.DataResetLabel.Size = new System.Drawing.Size(128, 25);
            this.DataResetLabel.TabIndex = 176;
            this.DataResetLabel.Text = "Data reset at:";
            // 
            // gbTimeOfLastDelete
            // 
            this.gbTimeOfLastDelete.Controls.Add(this.lblReceivedTime);
            this.gbTimeOfLastDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gbTimeOfLastDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTimeOfLastDelete.Location = new System.Drawing.Point(1299, 18);
            this.gbTimeOfLastDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbTimeOfLastDelete.Name = "gbTimeOfLastDelete";
            this.gbTimeOfLastDelete.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbTimeOfLastDelete.Size = new System.Drawing.Size(390, 65);
            this.gbTimeOfLastDelete.TabIndex = 170;
            this.gbTimeOfLastDelete.TabStop = false;
            this.gbTimeOfLastDelete.Text = "Last Data Received";
            this.gbTimeOfLastDelete.Visible = false;
            // 
            // lblReceivedTime
            // 
            this.lblReceivedTime.BackColor = System.Drawing.Color.Black;
            this.lblReceivedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReceivedTime.ForeColor = System.Drawing.Color.Red;
            this.lblReceivedTime.Location = new System.Drawing.Point(9, 26);
            this.lblReceivedTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReceivedTime.Name = "lblReceivedTime";
            this.lblReceivedTime.Size = new System.Drawing.Size(372, 31);
            this.lblReceivedTime.TabIndex = 0;
            this.lblReceivedTime.Text = "--";
            this.lblReceivedTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbTime
            // 
            this.gbTime.Controls.Add(this.timeOfDayLabel);
            this.gbTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gbTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbTime.Location = new System.Drawing.Point(10, 12);
            this.gbTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbTime.Name = "gbTime";
            this.gbTime.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbTime.Size = new System.Drawing.Size(396, 65);
            this.gbTime.TabIndex = 169;
            this.gbTime.TabStop = false;
            this.gbTime.Text = "Current Time";
            this.gbTime.Enter += new System.EventHandler(this.gbTime_Enter);
            // 
            // timeOfDayLabel
            // 
            this.timeOfDayLabel.BackColor = System.Drawing.Color.Black;
            this.timeOfDayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeOfDayLabel.ForeColor = System.Drawing.Color.Red;
            this.timeOfDayLabel.Location = new System.Drawing.Point(9, 26);
            this.timeOfDayLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.timeOfDayLabel.Name = "timeOfDayLabel";
            this.timeOfDayLabel.Size = new System.Drawing.Size(382, 31);
            this.timeOfDayLabel.TabIndex = 0;
            this.timeOfDayLabel.Text = "--";
            this.timeOfDayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // symbolDataGrid
            // 
            this.symbolDataGrid.ColumnHeadersHeight = 34;
            this.symbolDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.symbolDataGrid.Location = new System.Drawing.Point(295, 755);
            this.symbolDataGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.symbolDataGrid.MultiSelect = false;
            this.symbolDataGrid.Name = "symbolDataGrid";
            this.symbolDataGrid.ReadOnly = true;
            this.symbolDataGrid.RowHeadersWidth = 20;
            this.symbolDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.symbolDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.symbolDataGrid.Size = new System.Drawing.Size(562, 62);
            this.symbolDataGrid.TabIndex = 173;
            this.symbolDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(114, 1500);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(256, 60);
            this.button1.TabIndex = 174;
            this.button1.Text = "Get DB Table";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(70, 1504);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(256, 60);
            this.button2.TabIndex = 175;
            this.button2.Text = "Set Symbol List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(334, 1505);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(256, 60);
            this.button3.TabIndex = 177;
            this.button3.Text = "Get Dow 30 Data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(659, 1508);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(148, 55);
            this.button4.TabIndex = 179;
            this.button4.Text = "Test Email";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            // 
            // TODTimer
            // 
            this.TODTimer.Interval = 1000;
            this.TODTimer.Tick += new System.EventHandler(this.TODTimer_Tick);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(892, 1507);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(184, 58);
            this.button5.TabIndex = 180;
            this.button5.Text = "Disconnect";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(766, 1510);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(164, 55);
            this.button6.TabIndex = 181;
            this.button6.Text = "Pause";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(471, 1505);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(256, 60);
            this.button7.TabIndex = 182;
            this.button7.Text = "Update ZipperDataFile";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            // 
            // WatchdogTimer
            // 
            this.WatchdogTimer.Interval = 5000;
            this.WatchdogTimer.Tick += new System.EventHandler(this.WatchdogTimer_Tick);
            // 
            // ResetTimer
            // 
            this.ResetTimer.Interval = 60000;
            this.ResetTimer.Tick += new System.EventHandler(this.ResetTimer_Tick);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(1233, 1500);
            this.button8.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(185, 65);
            this.button8.TabIndex = 183;
            this.button8.Text = "Yesterday\" Close";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Visible = false;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(43, 1500);
            this.button9.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(256, 60);
            this.button9.TabIndex = 174;
            this.button9.Text = "Get DB Table";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 25);
            this.label2.TabIndex = 184;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 24);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 25);
            this.label3.TabIndex = 185;
            this.label3.Text = "label3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(20, 68);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 25);
            this.label5.TabIndex = 186;
            this.label5.Text = "label5";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(20, 146);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 25);
            this.label8.TabIndex = 187;
            this.label8.Text = "label8";
            // 
            // InitTimer
            // 
            this.InitTimer.Enabled = true;
            this.InitTimer.Interval = 60000;
            this.InitTimer.Tick += new System.EventHandler(this.InitTimer_Tick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(521, 34);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 25);
            this.label9.TabIndex = 188;
            this.label9.Text = "label9";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(281, 33);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 25);
            this.label10.TabIndex = 189;
            this.label10.Text = "label10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(281, 69);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 25);
            this.label11.TabIndex = 190;
            this.label11.Text = "label11";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(521, 69);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 25);
            this.label12.TabIndex = 191;
            this.label12.Text = "label12";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(20, 183);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 25);
            this.label13.TabIndex = 192;
            this.label13.Text = "label13";
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(9, 381);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(823, 219);
            this.groupBox3.TabIndex = 193;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Stats";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(521, 183);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 25);
            this.label14.TabIndex = 193;
            this.label14.Text = "label14";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // ReconnectTimer
            // 
            this.ReconnectTimer.Interval = 120000;
            this.ReconnectTimer.Tick += new System.EventHandler(this.ReconnectTimer_Tick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(9, 159);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(513, 58);
            this.groupBox4.TabIndex = 194;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "DB Table Name";
            // 
            // DailyResetLabel
            // 
            this.DailyResetLabel.AutoSize = true;
            this.DailyResetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DailyResetLabel.Location = new System.Drawing.Point(13, 664);
            this.DailyResetLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DailyResetLabel.Name = "DailyResetLabel";
            this.DailyResetLabel.Size = new System.Drawing.Size(128, 25);
            this.DailyResetLabel.TabIndex = 195;
            this.DailyResetLabel.Text = "Data reset at:";
            // 
            // ServerResetLabel
            // 
            this.ServerResetLabel.AutoSize = true;
            this.ServerResetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerResetLabel.Location = new System.Drawing.Point(13, 702);
            this.ServerResetLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ServerResetLabel.Name = "ServerResetLabel";
            this.ServerResetLabel.Size = new System.Drawing.Size(128, 25);
            this.ServerResetLabel.TabIndex = 196;
            this.ServerResetLabel.Text = "Data reset at:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(878, 840);
            this.Controls.Add(this.ServerResetLabel);
            this.Controls.Add(this.DailyResetLabel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.symbolDataGrid);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.DataResetLabel);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbTimeOfLastDelete);
            this.Controls.Add(this.gbTime);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmMain";
            this.Text = "TDF Dow 30  v1.5.3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbTimeOfLastDelete.ResumeLayout(false);
            this.gbTime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.symbolDataGrid)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblLogResp;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label PWlabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox PWTextBox;
        private System.Windows.Forms.Label Userlabel;
        private System.Windows.Forms.TextBox UserTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label IPlabel;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.GroupBox gbTimeOfLastDelete;
        private System.Windows.Forms.Label lblReceivedTime;
        private System.Windows.Forms.GroupBox gbTime;
        private System.Windows.Forms.Label timeOfDayLabel;
        private System.Windows.Forms.DataGridView symbolDataGrid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label DataResetLabel;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer TODTimer;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Timer WatchdogTimer;
        private System.Windows.Forms.Timer ResetTimer;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer InitTimer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Timer ReconnectTimer;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox ServerTextBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label DailyResetLabel;
        private System.Windows.Forms.Label ServerResetLabel;
    }
}

