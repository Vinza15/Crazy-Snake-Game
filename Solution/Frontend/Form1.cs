using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json; // WAJIB untuk save/load

namespace Frontend
{
    public partial class Form1 : Form
    {
        // --- Pengaturan Game ---
        private int segmentSize = 20;
        private List<Point> Snake;
        private Point Food;
        private string direction;
        private bool isGameOver;
        private int score;

        // --- Variabel Pembantu ---
        private Random rand = new Random();

        // --- Variabel Level Baru ---
        private List<LevelData> allLevels;
        private LevelData currentLevel;
        private int currentTargetScore;

        // --- Variabel User & Save ---
        private User loggedInUser;
        private SaveState currentUserSave;

        // --- Pengaturan API ---
        // GANTI "5238" DENGAN PORT ANDA JIKA BERBEDA
        private const string ApiUsersUrl = "http://localhost:5238/api/users";
        private const string ApiScoresUrl = "http://localhost:5238/api/scores";
        private const string ApiSaveStateUrl = "http://localhost:5238/api/savestate";
        private static readonly HttpClient apiClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            InitializeLevels(); // Muat data level
            LoadHighScores();   // Muat leaderboard saat start

            // Pastikan Form bisa "mendengar" tombol keyboard
            // Anda harus mengatur properti Form 'KeyPreview' = True di Desainer
            this.KeyPreview = true;
        }

        private void InitializeLevels()
        {
            allLevels = new List<LevelData>();

            // Level 1: Target 5, tidak ada obstacle
            allLevels.Add(new LevelData
            {
                LevelNumber = 1,
                TargetScore = 5,
                Obstacles = new List<Point>()
            });

            // Level 2: Target 10, obstacle di tengah
            allLevels.Add(new LevelData
            {
                LevelNumber = 2,
                TargetScore = 10,
                Obstacles = new List<Point>
                {
                    new Point(240, 200), new Point(260, 200), new Point(280, 200),
                    new Point(240, 220), new Point(260, 220), new Point(280, 220)
                }
            });

            // Level 3: Target 15, obstacle di pinggir
            allLevels.Add(new LevelData
            {
                LevelNumber = 3,
                TargetScore = 15,
                Obstacles = new List<Point>
                {
                    new Point(100, 100), new Point(100, 120), new Point(100, 140),
                    new Point(400, 300), new Point(400, 320), new Point(400, 340)
                }
            });

            // Level 4: Target 20, obstacle kotak
            allLevels.Add(new LevelData
            {
                LevelNumber = 4,
                TargetScore = 20,
                Obstacles = new List<Point>
                {
                    new Point(160, 160), new Point(180, 160), new Point(200, 160),
                    new Point(160, 180), new Point(200, 180),
                    new Point(160, 200), new Point(180, 200), new Point(200, 200)
                }
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

            currentTargetScore = currentLevel.TargetScore;
            gameTimer.Interval = 150 - (levelNumber * 10); // Game semakin cepat
            UpdateGameInfoLabel();
            GenerateFood();
        }

        private void UpdateGameInfoLabel()
        {
            if (currentLevel == null) return;
            lblGameInfo.Text = $"Level: {currentLevel.LevelNumber} | Skor: {score} / Target: {currentTargetScore}";
        }

        private void StartGame()
        {
            isGameOver = false;
            score = 0;
            direction = "right";
            Snake = new List<Point> { new Point(100, 100), new Point(80, 100), new Point(60, 100) };

            LoadLevel(1); // Muat level pertama

            gameTimer.Start();

            // Atur UI
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
            if (Snake == null || currentLevel == null) return; // Cek jika game belum siap

            int maxX = pbCanvas.Width / segmentSize;
            int maxY = pbCanvas.Height / segmentSize;
            bool foodOnObstacle;

            do
            {
                Food = new Point(rand.Next(0, maxX) * segmentSize,
                                 rand.Next(0, maxY) * segmentSize);

                // Cek jika makanan muncul di obstacle
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

            // Kirim skor
            await SendHighScore(loggedInUser.PlayerName, score, currentLevel.LevelNumber);
            // Hapus save data
            await DeleteSaveState();

            ResetUI();
        }

        private async void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            MessageBox.Show($"Game Over!\nSkor Anda: {score}", "Game Over");

            // Kirim skor (skor level terakhir yang dicapai)
            int levelReached = (currentLevel != null) ? currentLevel.LevelNumber : 1;
            await SendHighScore(loggedInUser.PlayerName, score, levelReached);

            // Hapus save data
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
            Snake = null; // Hentikan penggambaran
            currentLevel = null;
            isGameOver = true; // Set game over untuk menghentikan loop

            lblGameInfo.Text = "Silakan Login";
            pbCanvas.Invalidate(); // Bersihkan canvas
        }

        // --- Event Handler (Input, Timer, Paint) ---

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Hanya proses input jika game TIDAK game over
            if (isGameOver) return;

            // Cek input dan pastikan ular tidak bisa berbalik arah
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
            if (isGameOver) return;
            Point newHead = Snake[0];

            switch (direction)
            {
                case "right": newHead.X += segmentSize; break;
                case "left": newHead.X -= segmentSize; break;
                case "up": newHead.Y -= segmentSize; break;
                case "down": newHead.Y += segmentSize; break;
            }

            // --- Cek Tabrakan ---
            // 1. Dinding
            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= pbCanvas.Width || newHead.Y >= pbCanvas.Height)
            {
                GameOver(); return;
            }

            // 2. Diri Sendiri
            for (int i = 1; i < Snake.Count; i++) { if (newHead == Snake[i]) { GameOver(); return; } }

            // 3. Obstacle
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

            // --- Cek Makan & Naik Level ---
            if (newHead == Food)
            {
                score++;

                // Cek Naik Level
                if (score >= currentTargetScore)
                {
                    if (currentLevel.LevelNumber < allLevels.Count)
                    {
                        // Naik ke level berikutnya
                        LoadLevel(currentLevel.LevelNumber + 1);
                        score = 0; // Reset skor untuk level baru
                    }
                    else
                    {
                        // Tamat Game!
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
                // Bersihkan canvas jika game belum/tidak berjalan
                e.Graphics.Clear(pbCanvas.BackColor);
                return;
            }

            Graphics g = e.Graphics;

            if (!isGameOver)
            {
                // Gambar Makanan
                Rectangle foodRect = new Rectangle(Food.X, Food.Y, segmentSize, segmentSize);
                g.FillRectangle(Brushes.Red, foodRect);

                // Gambar Ular
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush segmentColor = (i == 0) ? Brushes.Green : Brushes.LimeGreen;
                    g.FillRectangle(segmentColor, Snake[i].X, Snake[i].Y, segmentSize, segmentSize);
                }

                // Gambar Obstacle
                foreach (Point obstacle in currentLevel.Obstacles)
                {
                    g.FillRectangle(Brushes.Gray, obstacle.X, obstacle.Y, segmentSize, segmentSize);
                }
            }
        }


        // --- Event Handler (API & Tombol Baru) ---

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
                // Coba Login
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

                // UI Update
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
            StartGame(); // Memulai dari Level 1

            pbCanvas.Focus();
        }

        private void btnContinueGame_Click(object sender, EventArgs e)
        {
            if (currentUserSave == null) return;

            // Muat data dari save state
            score = currentUserSave.Score;
            direction = currentUserSave.Direction;
            Snake = JsonSerializer.Deserialize<List<Point>>(currentUserSave.SnakeBodyJson);
            Food = JsonSerializer.Deserialize<Point>(currentUserSave.FoodPositionJson);

            // Muat level dan UI
            LoadLevel(currentUserSave.Level);

            // Mulai game
            isGameOver = false;
            gameTimer.Start();

            // Atur UI
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
            // 1. Hentikan permainan
            gameTimer.Stop();

            // 2. Simpan progres saat ini
            await SaveGameAsync();

            // 3. Tampilkan pesan (opsional)
            MessageBox.Show("Progres Anda telah disimpan.");

            // 4. Kembalikan ke menu utama
            ResetUI();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cek apakah game sedang berlangsung
            if (!isGameOver && loggedInUser != null)
            {
                // Panggil .Wait() untuk auto-save
                SaveGameAsync().Wait();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHighScores();
        }

        // --- Method Helper (API & Save) ---

        private async Task SaveGameAsync()
        {
            // Cek jika game sedang berlangsung
            if (isGameOver || loggedInUser == null || currentLevel == null)
            {
                // Jika game sudah selesai, kita tidak mau menyimpan.
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
                var scores = await apiClient.GetFromJsonAsync<List<HighScore>>(ApiScoresUrl);

                foreach (var scoreData in scores) // API sudah mengurutkan
                {
                    ListViewItem item = new ListViewItem(scoreData.PlayerName);
                    item.SubItems.Add(scoreData.Score.ToString());
                    item.SubItems.Add(scoreData.Level.ToString()); // TAMBAHKAN LEVEL
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