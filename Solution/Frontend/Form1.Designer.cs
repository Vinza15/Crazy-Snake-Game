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
            pbCanvas = new PictureBox();
            lblScore = new Label();
            btnStart = new Button();
            Nama = new Label();
            txtPlayerName = new TextBox();
            panelLeaderboard = new Panel();
            lvLeaderboard = new ListView();
            columnName = new ColumnHeader();
            columnScore = new ColumnHeader();
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
            panelGame.Controls.Add(pbCanvas);
            panelGame.Controls.Add(lblScore);
            panelGame.Controls.Add(btnStart);
            panelGame.Controls.Add(Nama);
            panelGame.Controls.Add(txtPlayerName);
            panelGame.Dock = DockStyle.Left;
            panelGame.Location = new Point(0, 0);
            panelGame.Name = "panelGame";
            panelGame.Size = new Size(466, 561);
            panelGame.TabIndex = 0;
            // 
            // pbCanvas
            // 
            pbCanvas.BackColor = SystemColors.AppWorkspace;
            pbCanvas.Location = new Point(11, 97);
            pbCanvas.Name = "pbCanvas";
            pbCanvas.Size = new Size(431, 439);
            pbCanvas.TabIndex = 4;
            pbCanvas.TabStop = false;
            pbCanvas.Click += pbCanvas_Click;
            pbCanvas.Paint += pbCanvas_Paint;
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblScore.Location = new Point(11, 59);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(62, 21);
            lblScore.TabIndex = 3;
            lblScore.Text = "Skor: 0";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(367, 11);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 2;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // Nama
            // 
            Nama.AutoSize = true;
            Nama.Location = new Point(11, 14);
            Nama.Name = "Nama";
            Nama.Size = new Size(45, 15);
            Nama.TabIndex = 1;
            Nama.Text = "Nama :";
            // 
            // txtPlayerName
            // 
            txtPlayerName.Location = new Point(62, 11);
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.Size = new Size(284, 23);
            txtPlayerName.TabIndex = 0;
            // 
            // panelLeaderboard
            // 
            panelLeaderboard.BorderStyle = BorderStyle.FixedSingle;
            panelLeaderboard.Controls.Add(lvLeaderboard);
            panelLeaderboard.Controls.Add(btnRefresh);
            panelLeaderboard.Controls.Add(label1);
            panelLeaderboard.Dock = DockStyle.Fill;
            panelLeaderboard.Location = new Point(466, 0);
            panelLeaderboard.Name = "panelLeaderboard";
            panelLeaderboard.Size = new Size(318, 561);
            panelLeaderboard.TabIndex = 1;
            // 
            // lvLeaderboard
            // 
            lvLeaderboard.Columns.AddRange(new ColumnHeader[] { columnName, columnScore });
            lvLeaderboard.Location = new Point(14, 46);
            lvLeaderboard.Name = "lvLeaderboard";
            lvLeaderboard.Size = new Size(291, 484);
            lvLeaderboard.TabIndex = 2;
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
            // btnRefresh
            // 
            btnRefresh.Dock = DockStyle.Bottom;
            btnRefresh.Location = new Point(0, 536);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(316, 23);
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
            label1.Location = new Point(55, 8);
            label1.Name = "label1";
            label1.Size = new Size(205, 25);
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
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(panelLeaderboard);
            Controls.Add(panelGame);
            KeyPreview = true;
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
        private TextBox txtPlayerName;
        private Label Nama;
        private Label lblScore;
        private Button btnStart;
        private PictureBox pbCanvas;
        private Label label1;
        private System.Windows.Forms.Timer gameTimer;
        private ListView lvLeaderboard;
        private Button btnRefresh;
        private ColumnHeader columnName;
        private ColumnHeader columnScore;
    }
}
