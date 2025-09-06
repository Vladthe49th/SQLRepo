using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class Target
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SuspicionLevel { get; set; } // (0-100)
        public bool IsAlive { get; set; } = true;

        public void Move()
        {
            // Movement/Activity simulation
        }
    }

}
