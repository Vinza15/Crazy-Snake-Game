namespace Backend
{
    public class SaveState
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Menghubungkan ke User.Id

        // Status Game
        public int Score { get; set; }
        public int Level { get; set; }
        public string Direction { get; set; }

        // Data game yang kompleks, disimpan sebagai string JSON
        public string SnakeBodyJson { get; set; }
        public string FoodPositionJson { get; set; }
    }
}
