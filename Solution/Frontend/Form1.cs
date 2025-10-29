using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; // Perlu package System.Net.Http.Json
using System.Threading.Tasks;
using System.Windows.Forms;

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

        // --- Pengaturan API ---
        // GANTI URL INI dengan URL API Anda yang sedang berjalan
        private const string ApiBaseUrl = "http://localhost:5238/api/scores";
        private static readonly HttpClient apiClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();

            // Nonaktifkan canvas sampai game dimulai
            pbCanvas.Enabled = false;

            // Muat leaderboard saat aplikasi pertama kali dibuka
            LoadHighScores();
        }

        private void StartGame()
        {
            isGameOver = false;
            score = 0;
            lblScore.Text = "Skor: 0";
            direction = "right"; // Mulai dengan bergerak ke kanan

            // Buat ular awal
            Snake = new List<Point>();
            Snake.Add(new Point(100, 100)); // Kepala
            Snake.Add(new Point(80, 100));
            Snake.Add(new Point(60, 100)); // Ekor

            GenerateFood();

            // Mulai "detak jantung" game
            gameTimer.Start();
        }

        private void GenerateFood()
        {
            int maxX = pbCanvas.Width / segmentSize;
            int maxY = pbCanvas.Height / segmentSize;

            // Buat lokasi acak
            do
            {
                Food = new Point(rand.Next(0, maxX) * segmentSize,
                                 rand.Next(0, maxY) * segmentSize);
            }
            // Pastikan makanan tidak muncul di atas tubuh ular
            while (Snake.Contains(Food));
        }

        private async void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();

            MessageBox.Show($"Game Over!\nSkor Anda: {score}", "Game Over");

            // Kirim skor ke API
            string playerName = txtPlayerName.Text;
            await SendHighScore(playerName, score);

            // Segarkan leaderboard setelah mengirim skor
            LoadHighScores();

            // Aktifkan kembali UI untuk game baru
            txtPlayerName.Enabled = true;
            btnStart.Enabled = true;
            pbCanvas.Enabled = false;
        }


        // --- Event Handler (Input, Timer, Paint) ---

        private void btnStart_Click(object sender, EventArgs e)
        {
            // 1. Validasi Nama
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                MessageBox.Show("Nama harus diisi sebelum memulai!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kunci Input Nama dan Tombol
            txtPlayerName.Enabled = false;
            btnStart.Enabled = false;
            pbCanvas.Enabled = true;

            // 3. Fokus ke Canvas agar bisa menerima input keyboard
            pbCanvas.Focus();

            // 4. Mulai Permainan!
            StartGame();
        }

        private void pbCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            // Cek input dan pastikan ular tidak bisa berbalik arah
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != "down") direction = "up";
                    break;
                case Keys.Down:
                    if (direction != "up") direction = "down";
                    break;
                case Keys.Left:
                    if (direction != "right") direction = "left";
                    break;
                case Keys.Right:
                    if (direction != "left") direction = "right";
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (isGameOver) return;

            // 1. Tentukan posisi kepala baru
            Point newHead = Snake[0];
            switch (direction)
            {
                case "right": newHead.X += segmentSize; break;
                case "left": newHead.X -= segmentSize; break;
                case "up": newHead.Y -= segmentSize; break;
                case "down": newHead.Y += segmentSize; break;
            }

            // 2. Periksa Tabrakan (Dinding)
            if (newHead.X < 0 || newHead.Y < 0 ||
                newHead.X >= pbCanvas.Width || newHead.Y >= pbCanvas.Height)
            {
                GameOver();
                return;
            }

            // 2. Periksa Tabrakan (Diri sendiri)
            for (int i = 1; i < Snake.Count; i++)
            {
                if (newHead == Snake[i])
                {
                    GameOver();
                    return;
                }
            }

            // 3. Tambahkan kepala baru
            Snake.Insert(0, newHead);

            // 4. Periksa apakah makan makanan
            if (newHead == Food)
            {
                score++;
                lblScore.Text = "Skor: " + score;
                GenerateFood();
            }
            else
            {
                // Hapus ekor
                Snake.RemoveAt(Snake.Count - 1);
            }

            // 5. Minta PictureBox untuk menggambar ulang
            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (Snake == null) return;
            Graphics g = e.Graphics;

            if (isGameOver) return; // Jangan gambar jika game over

            // Gambar Ular
            for (int i = 0; i < Snake.Count; i++)
            {
                Rectangle segment = new Rectangle(Snake[i].X, Snake[i].Y, segmentSize, segmentSize);
                Brush segmentColor = (i == 0) ? Brushes.Green : Brushes.LimeGreen;
                g.FillRectangle(segmentColor, segment);
            }

            // Gambar Makanan
            Rectangle foodRect = new Rectangle(Food.X, Food.Y, segmentSize, segmentSize);
            g.FillRectangle(Brushes.Red, foodRect);
        }


        // --- Event Handler (API) ---

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHighScores();
        }

        private async void LoadHighScores()
        {
            lvLeaderboard.Items.Clear();
            btnRefresh.Enabled = false;

            try
            {
                // Panggil API untuk GET data
                var scores = await apiClient.GetFromJsonAsync<List<HighScore>>(ApiBaseUrl);

                // Urutkan data (opsional, API mungkin sudah mengurutkan)
                var sortedScores = scores.OrderByDescending(s => s.Score).Take(10); // Ambil 10 teratas

                // Masukkan data ke ListView
                foreach (var scoreData in sortedScores)
                {
                    ListViewItem item = new ListViewItem(scoreData.PlayerName);
                    item.SubItems.Add(scoreData.Score.ToString());
                    lvLeaderboard.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data leaderboard. Pastikan API sudah berjalan.\nError: " + ex.Message, "Koneksi Gagal");
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }

        private async Task SendHighScore(string playerName, int score)
        {
            // Buat objek data untuk dikirim
            var scoreData = new HighScore { PlayerName = playerName, Score = score };

            try
            {
                // Kirim data sebagai JSON (POST)
                HttpResponseMessage response = await apiClient.PostAsJsonAsync(ApiBaseUrl, scoreData);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Gagal menyimpan skor ke server.", "API Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal terhubung ke server untuk menyimpan skor.\nError: " + ex.Message, "Koneksi Gagal");
            }
        }

        private void pbCanvas_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Hanya proses input jika game TIDAK game over
            if (isGameOver) return;

            // Cek input dan pastikan ular tidak bisa berbalik arah
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != "down") direction = "up";
                    break;
                case Keys.Down:
                    if (direction != "up") direction = "down";
                    break;
                case Keys.Left:
                    if (direction != "right") direction = "left";
                    break;
                case Keys.Right:
                    if (direction != "left") direction = "right";
                    break;
            }
        }
    }
}
