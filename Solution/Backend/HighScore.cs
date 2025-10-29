namespace Backend
{
    public class HighScore
    {
        public int Id { get; set; } // Kunci utama untuk database
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }
}
