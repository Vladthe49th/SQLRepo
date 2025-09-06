using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class World
    {
        public List<Target> Targets { get; set; } = new List<Target>();

        public void Simulate()
        {
            foreach (var target in Targets)
            {
                target.Move();
            }
        }
    }

}
