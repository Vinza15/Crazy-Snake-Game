namespace Frontend
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            panelGame = new Panel();
            btnExitToMenu = new Button();
            btnStartNewGame = new Button();
            btnContinueGame = new Button();
            btnLogin = new Button();
            txtPassword = new TextBox();
            Password = new Label();
            pbCanvas = new PictureBox();
            lblGameInfo = new Label();
            Username = new Label();
            txtUsername = new TextBox();
            panelLeaderboard = new Panel();
            lvLeaderboard = new ListView();
            columnName = new ColumnHeader();
            columnScore = new ColumnHeader();
            columnLevel = new ColumnHeader();
            btnRefresh = new Button();
            label1 = new Label();
            gameTimer = new System.Windows.Forms.Timer(components);
            panelGame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCanvas).BeginInit();
            panelLeaderboard.SuspendLayout();
            SuspendLayout();
            // 
            // panelGame
            // 
            panelGame.BorderStyle = BorderStyle.FixedSingle;
            panelGame.Controls.Add(btnExitToMenu);
            panelGame.Controls.Add(btnStartNewGame);
            panelGame.Controls.Add(btnContinueGame);
            panelGame.Controls.Add(btnLogin);
            panelGame.Controls.Add(txtPassword);
            panelGame.Controls.Add(Password);
            panelGame.Controls.Add(pbCanvas);
            panelGame.Controls.Add(lblGameInfo);
            panelGame.Controls.Add(Username);
            panelGame.Controls.Add(txtUsername);
            panelGame.Dock = DockStyle.Left;
            panelGame.Location = new Point(0, 0);
            panelGame.Margin = new Padding(3, 4, 3, 4);
            panelGame.Name = "panelGame";
            panelGame.Size = new Size(549, 775);
            panelGame.TabIndex = 0;
            // 
            // btnExitToMenu
            // 
            btnExitToMenu.Location = new Point(163, 723);
            btnExitToMenu.Margin = new Padding(3, 4, 3, 4);
            btnExitToMenu.Name = "btnExitToMenu";
            btnExitToMenu.Size = new Size(171, 31);
            btnExitToMenu.TabIndex = 10;
            btnExitToMenu.Text = "Keluar ke Menu";
            btnExitToMenu.UseVisualStyleBackColor = true;
            btnExitToMenu.Click += btnExitToMenu_Click;
            // 
            // btnStartNewGame
            // 
            btnStartNewGame.Enabled = false;
            btnStartNewGame.Location = new Point(249, 100);
            btnStartNewGame.Margin = new Padding(3, 4, 3, 4);
            btnStartNewGame.Name = "btnStartNewGame";
            btnStartNewGame.Size = new Size(93, 31);
            btnStartNewGame.TabIndex = 9;
            btnStartNewGame.Text = "Game Baru";
            btnStartNewGame.UseVisualStyleBackColor = true;
            btnStartNewGame.Click += btnStartNewGame_Click;
            // 
            // btnContinueGame
            // 
            btnContinueGame.Enabled = false;
            btnContinueGame.Location = new Point(128, 100);
            btnContinueGame.Margin = new Padding(3, 4, 3, 4);
            btnContinueGame.Name = "btnContinueGame";
            btnContinueGame.Size = new Size(86, 31);
            btnContinueGame.TabIndex = 8;
            btnContinueGame.Text = "Lanjutkan";
            btnContinueGame.UseVisualStyleBackColor = true;
            btnContinueGame.Click += btnContinueGame_Click;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(393, 35);
            btnLogin.Margin = new Padding(3, 4, 3, 4);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(134, 31);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "Login / Register";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(95, 55);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(285, 27);
            txtPassword.TabIndex = 6;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // Password
            // 
            Password.AutoSize = true;
            Password.Location = new Point(13, 59);
            Password.Name = "Password";
            Password.Size = new Size(77, 20);
            Password.TabIndex = 5;
            Password.Text = "Password :";
            // 
            // pbCanvas
            // 
            pbCanvas.BackColor = SystemColors.AppWorkspace;
            pbCanvas.Location = new Point(13, 180);
            pbCanvas.Margin = new Padding(3, 4, 3, 4);
            pbCanvas.Name = "pbCanvas";
            pbCanvas.Size = new Size(520, 520);
            pbCanvas.TabIndex = 4;
            pbCanvas.TabStop = false;
            pbCanvas.Paint += pbCanvas_Paint;
            // 
            // lblGameInfo
            // 
            lblGameInfo.AutoSize = true;
            lblGameInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblGameInfo.Location = new Point(13, 135);
            lblGameInfo.Name = "lblGameInfo";
            lblGameInfo.Size = new Size(279, 28);
            lblGameInfo.TabIndex = 3;
            lblGameInfo.Text = "Level: 1 | Skor: 0 / Target: ??";
            // 
            // Username
            // 
            Username.AutoSize = true;
            Username.Location = new Point(13, 19);
            Username.Name = "Username";
            Username.Size = new Size(82, 20);
            Username.TabIndex = 1;
            Username.Text = "Username :";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(95, 15);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(285, 27);
            txtUsername.TabIndex = 0;
            // 
            // panelLeaderboard
            // 
            panelLeaderboard.BorderStyle = BorderStyle.FixedSingle;
            panelLeaderboard.Controls.Add(lvLeaderboard);
            panelLeaderboard.Controls.Add(btnRefresh);
            panelLeaderboard.Controls.Add(label1);
            panelLeaderboard.Dock = DockStyle.Fill;
            panelLeaderboard.Location = new Point(549, 0);
            panelLeaderboard.Margin = new Padding(3, 4, 3, 4);
            panelLeaderboard.Name = "panelLeaderboard";
            panelLeaderboard.Size = new Size(347, 775);
            panelLeaderboard.TabIndex = 1;
            // 
            // lvLeaderboard
            // 
            lvLeaderboard.Columns.AddRange(new ColumnHeader[] { columnName, columnScore, columnLevel });
            lvLeaderboard.Location = new Point(16, 61);
            lvLeaderboard.Margin = new Padding(3, 4, 3, 4);
            lvLeaderboard.Name = "lvLeaderboard";
            lvLeaderboard.Size = new Size(332, 667);
            lvLeaderboard.TabIndex = 2;
            lvLeaderboard.TabStop = false;
            lvLeaderboard.UseCompatibleStateImageBehavior = false;
            lvLeaderboard.View = View.Details;
            // 
            // columnName
            // 
            columnName.Text = "Nama";
            columnName.Width = 150;
            // 
            // columnScore
            // 
            columnScore.Text = "Score";
            columnScore.Width = 80;
            // 
            // columnLevel
            // 
            columnLevel.Text = "Level";
            // 
            // btnRefresh
            // 
            btnRefresh.Dock = DockStyle.Bottom;
            btnRefresh.Location = new Point(0, 738);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(345, 35);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label1.Location = new Point(54, 11);
            label1.Name = "label1";
            label1.Size = new Size(243, 30);
            label1.TabIndex = 0;
            label1.Text = "🏆 LEADERBOARD 🏆";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // gameTimer
            // 
            gameTimer.Interval = 150;
            gameTimer.Tick += gameTimer_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(896, 775);
            Controls.Add(panelLeaderboard);
            Controls.Add(panelGame);
            KeyPreview = true;
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Crazy Snake";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            panelGame.ResumeLayout(false);
            panelGame.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbCanvas).EndInit();
            panelLeaderboard.ResumeLayout(false);
            panelLeaderboard.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelGame;
        private Panel panelLeaderboard;
        private TextBox txtUsername;
        private Label Username;
        private Label lblGameInfo;
        private PictureBox pbCanvas;
        private Label label1;
        private System.Windows.Forms.Timer gameTimer;
        private ListView lvLeaderboard;
        private Button btnRefresh;
        private ColumnHeader columnName;
        private ColumnHeader columnScore;
        private TextBox txtPassword;
        private Label Password;
        private Button btnStartNewGame;
        private Button btnContinueGame;
        private Button btnLogin;
        private Button btnExitToMenu;
        private ColumnHeader columnLevel;
    }
}
