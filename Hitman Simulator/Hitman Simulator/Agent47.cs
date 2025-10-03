using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{

    public class Agent47 : IAgent
    {
       
        public string Codename { get; private set; } = "Agent 47";


        public string Name => Codename;

        public void Observe(ITarget target)
        {
            if (target == null) return;
            Logger.Instance.Log($"{Name} is observing {target.Name} (Suspicion {target.SuspicionLevel})");

        }

        public void Eliminate(ITarget target)
        {
            if (target == null || !target.IsAlive) return;

            target.Eliminate(); 
            Logger.Instance.Log($"{Name} eliminated {target.Name}");
        }
    }


}
