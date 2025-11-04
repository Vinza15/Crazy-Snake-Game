using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    using System.Collections.Generic;
    using System.Drawing;

    public class LevelData
    {
        public int LevelNumber { get; set; }
        public int TargetScore { get; set; }
        public List<Point> Obstacles { get; set; }
    }
}
