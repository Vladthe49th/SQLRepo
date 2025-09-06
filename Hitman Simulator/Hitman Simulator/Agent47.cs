using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hitman_Simulator
{
    public class Agent47
    {
        public string Codename { get; set; } = "Agent 47";

        public void Observe(Target target)
        {
            // Observe target
        }

        public void Eliminate(Target target)
        {
            if (target.IsAlive)
            {
                target.IsAlive = false;
                Console.WriteLine($"{Codename} eliminated {target.Name}");
            }
        }
    }

}
