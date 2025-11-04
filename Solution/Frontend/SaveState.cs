using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    public class SaveState
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public string Direction { get; set; }
        public string SnakeBodyJson { get; set; }
        public string FoodPositionJson { get; set; }
    }
}
