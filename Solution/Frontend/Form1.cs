using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;

namespace Frontend
{
    public partial class Form1 : Form
    {
        private int segmentSize = 20;
        private List<Point> Snake;
        private Point Food;
        private string direction;
        private bool isGameOver;
        private int score;

        private Random rand = new Random();

        private List<LevelData> allLevels;
        private LevelData currentLevel;
        private int cumulativeTargetScore;
        private bool isPausedForLevelUp = false;

        private User loggedInUser;
        private SaveState currentUserSave;

        private Image headUp;
        private Image headDown;
        private Image headLeft;
        private Image headRight;
        private Image bodyAssetVertical;
        private Image bodyAssetHorizontal;
        private Image foodAsset;
        private Image obstacleAsset;

        private const string ApiUsersUrl = "http://localhost:5238/api/users";
        private const string ApiScoresUrl = "http://localhost:5238/api/scores";
        private const string ApiSaveStateUrl = "http://localhost:5238/api/savestate";
        private static readonly HttpClient apiClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            InitializeLevels();
            LoadHighScores();
            this.KeyPreview = true;

            try
            {
                headUp = Image.FromFile("Resources/head_up.png");
                headDown = Image.FromFile("Resources/head_down.png");
                headLeft = Image.FromFile("Resources/head_left.png");
                headRight = Image.FromFile("Resources/head_right.png");
                bodyAssetVertical = Image.FromFile("Resources/body_vertical.png");
                bodyAssetHorizontal = Image.FromFile("Resources/body_horizontal.png");
                foodAsset = Image.FromFile("Resources/food.png");
                obstacleAsset = Image.FromFile("Resources/obstacle.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat aset gambar. Pastikan file ada di folder Resources. Error: " + ex.Message);
            }
        }

        private void InitializeLevels()
        {
            allLevels = new List<LevelData>();

            allLevels.Add(new LevelData
            {
                LevelNumber = 1,
                TargetScore = 8,
                Obstacles = BuildLevelFromImage("Resources/level_1.png")
            });

            allLevels.Add(new LevelData
            {
                LevelNumber = 2,
                TargetScore = 10,
                Obstacles = BuildLevelFromImage("Resources/level_2.png")
            });

            allLevels.Add(new LevelData
            {
                LevelNumber = 3,
                TargetScore = 12,
                Obstacles = BuildLevelFromImage("Resources/level_3.png")
            });
        }

        private void LoadLevel(int levelNumber)
        {
            currentLevel = allLevels.FirstOrDefault(lvl => lvl.LevelNumber == levelNumber);

            if (currentLevel == null)
            {
                MessageBox.Show("Level tidak ditemukan!");
                return;
            }

            // Hitung total skor target akumulatif
            cumulativeTargetScore = 0;
            for (int i = 0; i < currentLevel.LevelNumber; i++)
            {
                cumulativeTargetScore += allLevels[i].TargetScore;
            }

            gameTimer.Interval = 150 - (levelNumber * 10);
            UpdateGameInfoLabel();
            GenerateFood();

            // Jeda permainan jika ini bukan level 1
            if (levelNumber > 1)
            {
                isPausedForLevelUp = true;
                lblGameInfo.Text = $"Level: {currentLevel.LevelNumber} | Skor: {score} / Target: {cumulativeTargetScore} | Tekan WASD untuk Lanjut!";
                pbCanvas.Invalidate();
            }
        }

        private void UpdateGameInfoLabel()
        {
            if (currentLevel == null) return;
            lblGameInfo.Text = $"Level: {currentLevel.LevelNumber} | Skor: {score} / Target: {cumulativeTargetScore}";
        }

        private void StartGame()
        {
            isGameOver = false;
            score = 0; // Skor direset HANYA saat game baru
            direction = "right";
            Snake = new List<Point> { new Point(100, 100), new Point(80, 100), new Point(60, 100) };
            isPausedForLevelUp = false; // Pastikan tidak dijeda di awal

            LoadLevel(1);

            gameTimer.Start();

            btnExitToMenu.Visible = true;
            btnExitToMenu.TabStop = false;
            btnRefresh.TabStop = false;
            btnLogin.Enabled = false;
            btnStartNewGame.Enabled = false;
            btnContinueGame.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
        }

        private void GenerateFood()
        {
            if (Snake == null || currentLevel == null) return;

            int maxX = pbCanvas.Width / segmentSize;
            int maxY = pbCanvas.Height / segmentSize;
            bool foodOnObstacle;

            do
            {
                Food = new Point(rand.Next(0, maxX) * segmentSize,
                                   rand.Next(0, maxY) * segmentSize);

                foodOnObstacle = false;
                foreach (Point obstacle in currentLevel.Obstacles)
                {
                    if (Food == obstacle)
                    {
                        foodOnObstacle = true;
                        break;
                    }
                }
            }
            // Ulangi jika makanan muncul di atas ular ATAU di atas obstacle
            while (Snake.Contains(Food) || foodOnObstacle);
        }

        private async void GameWin()
        {
            gameTimer.Stop();
            isGameOver = true;
            MessageBox.Show("Selamat! Anda telah menamatkan game!");

            await SendHighScore(loggedInUser.PlayerName, score, currentLevel.LevelNumber);
            await DeleteSaveState();
            ResetUI();
        }

        private async void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            MessageBox.Show($"Game Over!\nSkor Anda: {score}", "Game Over");

            int levelReached = (currentLevel != null) ? currentLevel.LevelNumber : 1;
            await SendHighScore(loggedInUser.PlayerName, score, levelReached);

            await DeleteSaveState();
            ResetUI();
        }

        private void ResetUI()
        {
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            btnLogin.Enabled = true;
            btnStartNewGame.Enabled = false;
            btnContinueGame.Enabled = false;
            btnExitToMenu.Visible = false;
            btnLogin.TabStop = true;
            btnStartNewGame.TabStop = true;
            btnContinueGame.TabStop = true;
            btnRefresh.TabStop = true;

            loggedInUser = null;
            currentUserSave = null;
            Snake = null;
            currentLevel = null;
            isGameOver = true;
            isPausedForLevelUp = false; // Reset status jeda

            lblGameInfo.Text = "Silakan Login";
            pbCanvas.Invalidate();
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Logika untuk "membangunkan" dari jeda
            if (isPausedForLevelUp)
            {
                // Hanya tombol WASD yang bisa memulai game lagi
                if (e.KeyCode == Keys.W || e.KeyCode == Keys.S || e.KeyCode == Keys.A || e.KeyCode == Keys.D)
                {
                    isPausedForLevelUp = false;
                    gameTimer.Start();
                    UpdateGameInfoLabel();
                }
                else
                {
                    // Abaikan tombol lain
                    e.Handled = true;
                    return;
                }
            }

            if (isGameOver) return;

            // Mencegah ular berbalik arah
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (direction != "down") direction = "up";
                    e.Handled = true;
                    break;
                case Keys.S:
                    if (direction != "up") direction = "down";
                    e.Handled = true;
                    break;
                case Keys.A:
                    if (direction != "right") direction = "left";
                    e.Handled = true;
                    break;
                case Keys.D:
                    if (direction != "left") direction = "right";
                    e.Handled = true;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Cek jika game sedang dijeda
            if (isPausedForLevelUp)
            {
                gameTimer.Stop();
                return;
            }

            if (isGameOver) return;
            Point newHead = Snake[0];

            switch (direction)
            {
                case "right": newHead.X += segmentSize; break;
                case "left": newHead.X -= segmentSize; break;
                case "up": newHead.Y -= segmentSize; break;
                case "down": newHead.Y += segmentSize; break;
            }

            // Cek Tabrakan Dinding
            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= pbCanvas.Width || newHead.Y >= pbCanvas.Height)
            {
                GameOver(); return;
            }

            // Cek Tabrakan Diri Sendiri
            for (int i = 1; i < Snake.Count; i++) { if (newHead == Snake[i]) { GameOver(); return; } }

            // Cek Tabrakan Obstacle
            if (currentLevel != null)
            {
                foreach (Point obstacle in currentLevel.Obstacles)
                {
                    if (newHead == obstacle)
                    {
                        GameOver(); return;
                    }
                }
            }

            Snake.Insert(0, newHead);

            if (newHead == Food)
            {
                score++; // Skor terus bertambah (akumulatif)

                // Cek skor akumulatif
                if (score >= cumulativeTargetScore)
                {
                    // Cek apakah ini level terakhir
                    if (currentLevel.LevelNumber < allLevels.Count)
                    {
                        LoadLevel(currentLevel.LevelNumber + 1);
                        // Skor tidak direset
                    }
                    else
                    {
                        GameWin();
                    }
                }
                GenerateFood();
            }
            else
            {
                Snake.RemoveAt(Snake.Count - 1); // Hapus ekor
            }

            UpdateGameInfoLabel();
            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (Snake == null || currentLevel == null)
            {
                e.Graphics.Clear(pbCanvas.BackColor);
                return;
            }

            Graphics g = e.Graphics;

            if (!isGameOver)
            {
                Rectangle foodRect = new Rectangle(Food.X, Food.Y, segmentSize, segmentSize);
                DrawImageContained(g, foodAsset, foodRect, pbCanvas.BackColor);

                for (int i = 0; i < Snake.Count; i++)
                {
                    Rectangle segmentRect = new Rectangle(Snake[i].X, Snake[i].Y, segmentSize, segmentSize);

                    if (i == 0) // Kepala
                    {
                        Image headImage = null;
                        switch (direction)
                        {
                            case "up": headImage = headUp; break;
                            case "down": headImage = headDown; break;
                            case "left": headImage = headLeft; break;
                            case "right": headImage = headRight; break;
                        }

                        if (headImage != null)
                        {
                            g.DrawImage(headImage, segmentRect);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.Green, segmentRect); // Fallback
                        }
                    }
                    else // Tubuh
                    {
                        Point segmentCurrent = Snake[i];
                        Point segmentInFront = Snake[i - 1];

                        Image assetToUse;
                        if (segmentCurrent.X == segmentInFront.X)
                        {
                            assetToUse = bodyAssetVertical;
                        }
                        else
                        {
                            assetToUse = bodyAssetHorizontal;
                        }

                        DrawImageContained(g, assetToUse, segmentRect, pbCanvas.BackColor);
                    }
                }

                foreach (Point obstacle in currentLevel.Obstacles)
                {
                    Rectangle obstacleRect = new Rectangle(obstacle.X, obstacle.Y, segmentSize, segmentSize);
                    DrawImageContained(g, obstacleAsset, obstacleRect, pbCanvas.BackColor);
                }
            }
        }

        /**
         * Menggambar 'image' di dalam 'container' dengan rasio aspek "Contain",
         * dan mengisi latar belakang (pillarbox/letterbox) dengan 'backgroundColor'.
         */
        private void DrawImageContained(Graphics g, Image image, Rectangle container, Color backgroundColor)
        {
            if (image == null)
            {
                // Gambar kotak fallback jika aset tidak ada
                using (SolidBrush fallbackBrush = new SolidBrush(Color.Magenta))
                {
                    g.FillRectangle(fallbackBrush, container);
                }
                return;
            }

            float imageRatio = (float)image.Width / image.Height;
            float containerRatio = (float)container.Width / container.Height;
            Rectangle destRect = new Rectangle();

            if (imageRatio < containerRatio)
            {
                // Kasus Aset Vertikal
                destRect.Height = container.Height;
                destRect.Width = (int)(container.Height * imageRatio);
            }
            else
            {
                // Kasus Aset Horizontal
                destRect.Width = container.Width;
                destRect.Height = (int)(container.Width / imageRatio);
            }

            destRect.X = container.X + (container.Width - destRect.Width) / 2;
            destRect.Y = container.Y + (container.Height - destRect.Height) / 2;

            using (SolidBrush bgBrush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(bgBrush, container);
            }
            g.DrawImage(image, destRect);
        }

        /**
         * Membaca file gambar (misal: 26x26 pixel) dan mengubahnya
         * menjadi daftar koordinat obstacle untuk game (misal: 520x520).
         */
        private List<Point> BuildLevelFromImage(string imagePath)
        {
            var obstacles = new List<Point>();
            Bitmap levelMap;

            try
            {
                levelMap = new Bitmap(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat file Peta Level: {imagePath}. Error: {ex.Message}");
                return obstacles;
            }

            for (int x = 0; x < levelMap.Width; x++)
            {
                for (int y = 0; y < levelMap.Height; y++)
                {
                    Color pixelColor = levelMap.GetPixel(x, y);
                    // Cek apakah pixelnya berwarna HITAM PEKAT
                    if (pixelColor.A > 0 && pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                    {
                        // Ubah koordinat pixel (misal: x=5) menjadi koordinat game (x=100)
                        obstacles.Add(new Point(x * segmentSize, y * segmentSize));
                    }
                }
            }
            levelMap.Dispose(); // Bebaskan memori gambar
            return obstacles;
        }


        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan Password harus diisi.");
                return;
            }

            var loginData = new User { PlayerName = username, Password = password };
            HttpResponseMessage response;

            try
            {
                response = await apiClient.PostAsJsonAsync($"{ApiUsersUrl}/login", loginData);

                if (!response.IsSuccessStatusCode)
                {
                    // Jika login gagal, coba Register
                    response = await apiClient.PostAsJsonAsync($"{ApiUsersUrl}/register", loginData);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Login dan Register gagal. Cek API atau password mungkin salah.");
                        return;
                    }
                }

                loggedInUser = await response.Content.ReadFromJsonAsync<User>();
                MessageBox.Show($"Selamat datang, {loggedInUser.PlayerName}!");

                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
                btnLogin.Enabled = false;
                btnStartNewGame.Enabled = true;

                // Cek apakah ada save data
                response = await apiClient.GetAsync($"{ApiSaveStateUrl}/{loggedInUser.Id}");
                if (response.IsSuccessStatusCode)
                {
                    currentUserSave = await response.Content.ReadFromJsonAsync<SaveState>();
                    btnContinueGame.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal terhubung ke server: " + ex.Message);
            }
        }

        private void btnStartNewGame_Click(object sender, EventArgs e)
        {
            StartGame();
            pbCanvas.Focus();
        }

        private void btnContinueGame_Click(object sender, EventArgs e)
        {
            if (currentUserSave == null) return;

            score = currentUserSave.Score;
            direction = currentUserSave.Direction;
            Snake = JsonSerializer.Deserialize<List<Point>>(currentUserSave.SnakeBodyJson);
            Food = JsonSerializer.Deserialize<Point>(currentUserSave.FoodPositionJson);

            LoadLevel(currentUserSave.Level);

            isGameOver = false;
            gameTimer.Start();

            btnExitToMenu.Visible = true;
            btnExitToMenu.TabStop = false;
            btnRefresh.TabStop = false;
            btnLogin.Enabled = false;
            btnStartNewGame.Enabled = false;
            btnContinueGame.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;

            pbCanvas.Focus();
        }

        private async void btnExitToMenu_Click(object sender, EventArgs e)
        {
            gameTimer.Stop();
            await SaveGameAsync();
            MessageBox.Show("Progres Anda telah disimpan.");
            ResetUI();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Auto-save jika game sedang berlangsung
            if (!isGameOver && loggedInUser != null)
            {
                SaveGameAsync().Wait();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHighScores();
        }

        private async Task SaveGameAsync()
        {
            // Jangan simpan jika game sudah selesai
            if (isGameOver || loggedInUser == null || currentLevel == null)
            {
                await DeleteSaveState();
                return;
            }

            var saveState = new SaveState
            {
                UserId = loggedInUser.Id,
                Score = this.score,
                Level = this.currentLevel.LevelNumber,
                Direction = this.direction,
                SnakeBodyJson = JsonSerializer.Serialize(this.Snake),
                FoodPositionJson = JsonSerializer.Serialize(this.Food)
            };
            await apiClient.PostAsJsonAsync(ApiSaveStateUrl, saveState);
        }

        private async Task DeleteSaveState()
        {
            if (loggedInUser == null) return;
            await apiClient.DeleteAsync($"{ApiSaveStateUrl}/{loggedInUser.Id}");
            currentUserSave = null;
        }

        private async Task SendHighScore(string playerName, int score, int level)
        {
            try
            {
                var scoreData = new HighScore { PlayerName = playerName, Score = score, Level = level };
                await apiClient.PostAsJsonAsync(ApiScoresUrl, scoreData);
                LoadHighScores(); // Refresh leaderboard
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan skor: " + ex.Message);
            }
        }

        private async void LoadHighScores()
        {
            lvLeaderboard.Items.Clear();
            btnRefresh.Enabled = false;

            try
            {
                // 1. Ambil SEMUA data skor dari API
                var allScores = await apiClient.GetFromJsonAsync<List<HighScore>>(ApiScoresUrl);

                if (allScores == null || !allScores.Any())
                {
                    return; // Tidak ada data untuk diproses
                }

                // 2. Gunakan LINQ untuk memfilter skor terbaik per pemain
                var bestScoresPerUser = allScores
                    .GroupBy(s => s.PlayerName) // Kelompokkan berdasarkan nama
                    .Select(g => g.OrderByDescending(s => s.Score) // Urutkan skor grup (tertinggi dulu)
                                  .ThenByDescending(s => s.Level) // Jika skor sama, urutkan level
                                  .First()); // Ambil HANYA yang pertama (terbaik)

                // 3. Urutkan hasil akhir leaderboard
                var finalLeaderboard = bestScoresPerUser
                    .OrderByDescending(s => s.Score)
                    .ThenByDescending(s => s.Level);

                // 4. Tampilkan di ListView
                foreach (var scoreData in finalLeaderboard)
                {
                    ListViewItem item = new ListViewItem(scoreData.PlayerName);
                    item.SubItems.Add(scoreData.Score.ToString());
                    item.SubItems.Add(scoreData.Level.ToString());
                    lvLeaderboard.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data leaderboard: " + ex.Message);
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}