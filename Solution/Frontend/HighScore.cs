using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    internal class HighScore
    {
        public int Id { get; set; } // Kunci utama untuk database
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }
}
